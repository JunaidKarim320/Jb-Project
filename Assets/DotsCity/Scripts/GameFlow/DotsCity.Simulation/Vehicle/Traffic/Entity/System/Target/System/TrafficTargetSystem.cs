using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Car;
using Spirit604.DotsCity.Simulation.Level.Streaming;
using Spirit604.DotsCity.Simulation.Road;
using Spirit604.DotsCity.Simulation.TrafficArea;
using Spirit604.Extensions;
using Spirit604.Gameplay.Road;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Traffic
{
    [UpdateInGroup(typeof(TrafficLateSimulationGroup))]
    [BurstCompile]
    public partial struct TrafficTargetSystem : ISystem
    {
        private EntityQuery updateQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            updateQuery = SystemAPI.QueryBuilder()
                .WithNone<TrafficCustomDestinationComponent, TrafficNoTargetTag, TrafficCustomTargetingTag>()
                .WithDisabled<TrafficChangingLaneEventTag>()
                .WithDisabledRW<TrafficSwitchTargetNodeRequestTag>()
                .WithAllRW<SpeedComponent, TrafficDestinationComponent>()
                .WithAllRW<TrafficPathComponent, TrafficStateComponent>()
                .WithPresentRW<TrafficEnteredTriggerNodeTag, TrafficEnteringTriggerNodeTag>()
                .WithPresentRW<TrafficIdleTag>()
                .WithAll<TrafficTag, HasDriverTag, TrafficTypeComponent, LocalTransform>()
                .Build();

            state.RequireForUpdate(updateQuery);
            state.RequireForUpdate<TrafficNodeResolverSystem.RuntimePathDataRef>();
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var targetJob = new TargetJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                TrafficAreaNodeLookup = SystemAPI.GetComponentLookup<TrafficAreaNode>(true),
                CapacityLookup = SystemAPI.GetComponentLookup<TrafficNodeCapacityComponent>(true),
                NodeSettingsLookup = SystemAPI.GetComponentLookup<TrafficNodeSettingsComponent>(true),
                WorldTransformLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true),
                InViewOfCameraLookup = SystemAPI.GetComponentLookup<InViewOfCameraTag>(true),
                TrafficRailMovementLookup = SystemAPI.GetComponentLookup<TrafficRailMovementTag>(true),
                PathConnectionLookup = SystemAPI.GetBufferLookup<PathConnectionElement>(true),
                Graph = SystemAPI.GetSingleton<PathGraphSystem.Singleton>(),
                TrafficNavConfig = SystemAPI.GetSingleton<TrafficDestinationConfigReference>(),
                ParkingConfig = SystemAPI.GetSingleton<TrafficParkingConfigReference>(),
                TrafficChangeLaneConfigReference = SystemAPI.GetSingleton<TrafficChangeLaneConfigReference>(),
                TrafficCommonSettingsConfigBlobReference = SystemAPI.GetSingleton<TrafficCommonSettingsConfigBlobReference>(),
                TrafficGeneralSettingsReference = SystemAPI.GetSingleton<TrafficGeneralSettingsReference>(),
                TrafficDestinationConfigReference = SystemAPI.GetSingleton<TrafficDestinationConfigReference>(),
                RuntimePathDataRef = SystemAPI.GetSingleton<TrafficNodeResolverSystem.RuntimePathDataRef>(),
            };

            targetJob.ScheduleParallel(updateQuery);
        }

        [WithNone(typeof(TrafficCustomDestinationComponent))]
        [WithDisabled(typeof(TrafficChangingLaneEventTag))]
        [WithAll(typeof(TrafficTag), typeof(HasDriverTag))]
        [BurstCompile]
        private partial struct TargetJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter CommandBuffer;

            [ReadOnly]
            public ComponentLookup<TrafficAreaNode> TrafficAreaNodeLookup;

            [ReadOnly]
            public ComponentLookup<TrafficNodeCapacityComponent> CapacityLookup;

            [ReadOnly]
            public ComponentLookup<TrafficNodeSettingsComponent> NodeSettingsLookup;

            [ReadOnly]
            public ComponentLookup<LocalToWorld> WorldTransformLookup;

            [ReadOnly]
            public ComponentLookup<InViewOfCameraTag> InViewOfCameraLookup;

            [ReadOnly]
            public ComponentLookup<TrafficRailMovementTag> TrafficRailMovementLookup;

            [ReadOnly]
            public BufferLookup<PathConnectionElement> PathConnectionLookup;

            [ReadOnly]
            public PathGraphSystem.Singleton Graph;

            [ReadOnly]
            public TrafficDestinationConfigReference TrafficNavConfig;

            [ReadOnly]
            public TrafficParkingConfigReference ParkingConfig;

            [ReadOnly]
            public TrafficChangeLaneConfigReference TrafficChangeLaneConfigReference;

            [ReadOnly]
            public TrafficCommonSettingsConfigBlobReference TrafficCommonSettingsConfigBlobReference;

            [ReadOnly]
            public TrafficGeneralSettingsReference TrafficGeneralSettingsReference;

            [ReadOnly]
            public TrafficDestinationConfigReference TrafficDestinationConfigReference;

            [ReadOnly]
            public TrafficNodeResolverSystem.RuntimePathDataRef RuntimePathDataRef;

            void Execute(
                Entity entity,
                [ChunkIndexInQuery] int entityInQueryIndex,
                ref SpeedComponent speedComponent,
                ref TrafficDestinationComponent destinationComponent,
                ref TrafficPathComponent trafficPathComponent,
                ref TrafficStateComponent trafficStateComponent,
                EnabledRefRW<TrafficEnteredTriggerNodeTag> trafficEnteredTriggerNodeTagRW,
                EnabledRefRW<TrafficEnteringTriggerNodeTag> trafficEnteringTriggerNodeTagRW,
                EnabledRefRW<TrafficSwitchTargetNodeRequestTag> trafficSwitchTargetNodeRequestTagRW,
                EnabledRefRW<TrafficIdleTag> trafficIdleTagRW,
                in TrafficTypeComponent trafficTypeComponent,
                in LocalTransform transform)
            {
                float3 carPosition = transform.Position;
                float3 currentTargetPosition = destinationComponent.Destination;

                ref readonly var pathData = ref Graph.GetPathData(trafficPathComponent.CurrentGlobalPathIndex);

                var dstNode = destinationComponent.DestinationNode;
                var hasDstNode = NodeSettingsLookup.HasComponent(dstNode);

                float distanceToTarget = math.distance(currentTargetPosition, carPosition);
                CheckDistanceHowFarPreviousTrafficLight(ref TrafficNavConfig, in WorldTransformLookup, ref destinationComponent, in transform);

                if (destinationComponent.PathConnectionType != PathConnectionType.PathPoint)
                {
                    var endPosition = Graph.GetEndPosition(in pathData);
                    float distanceToTargetNode = math.distance(endPosition, carPosition);

                    destinationComponent.DistanceToEndOfPath = distanceToTargetNode;

                    var nextNodeRequest = CheckIfNewTrafficNodeIsCloseEnough(ref TrafficNavConfig, ref destinationComponent, distanceToTargetNode);

                    if (nextNodeRequest)
                    {
                        if (!hasDstNode)
                        {
                            // Destination node unloaded due to road streaming
                            TrafficNoTargetUtils.AddNoTarget(ref CommandBuffer, entity, entityInQueryIndex, ref trafficStateComponent, ref trafficIdleTagRW, in TrafficDestinationConfigReference);
                            return;
                        }

                        if (destinationComponent.NextDestinationNode == Entity.Null)
                        {
                            CommandBuffer.SetComponentEnabled<TrafficNextTrafficNodeRequestTag>(entityInQueryIndex, entity, true);
                        }
                    }
                }
                else
                {
                    destinationComponent.DistanceToEndOfPath = math.distance(destinationComponent.Destination, carPosition);
                    destinationComponent.DistanceToWaypoint = destinationComponent.DistanceToEndOfPath;
                }

                float checkDistanceToTarget = 0;

                switch (destinationComponent.PathConnectionType)
                {
                    case PathConnectionType.TrafficNode:
                        checkDistanceToTarget = !hasDstNode || NodeSettingsLookup[dstNode].CustomAchieveDistance == 0 ? TrafficNavConfig.Config.Value.MinDistanceToTarget : NodeSettingsLookup[dstNode].CustomAchieveDistance;
                        break;
                    case PathConnectionType.PathPoint:
                        checkDistanceToTarget = TrafficNavConfig.Config.Value.MinDistanceToPathPointTarget;
                        break;
                }

                bool targetIsAchieved = distanceToTarget < checkDistanceToTarget;

                if (targetIsAchieved && destinationComponent.PathConnectionType == PathConnectionType.PathPoint)
                {
                    if (trafficPathComponent.LocalPathNodeIndex < pathData.NodeCount - 1)
                    {
                        targetIsAchieved = false;
                    }
                }

                bool switchToNextTarget = targetIsAchieved;

                if (switchToNextTarget)
                {
                    if (!hasDstNode)
                    {
                        // Destination node unloaded due to road streaming
                        TrafficNoTargetUtils.AddNoTarget(ref CommandBuffer, entity, entityInQueryIndex, ref trafficStateComponent, ref trafficIdleTagRW, in TrafficDestinationConfigReference);
                        return;
                    }

                    TrafficTargetUtils.ProcessAchievedTarget(
                        ref CommandBuffer,
                        entity,
                        entityInQueryIndex,
                        ref destinationComponent,
                        ref trafficStateComponent,
                        ref trafficEnteredTriggerNodeTagRW,
                        ref trafficEnteringTriggerNodeTagRW,
                        ref trafficSwitchTargetNodeRequestTagRW,
                        ref trafficIdleTagRW,
                        in NodeSettingsLookup,
                        in CapacityLookup,
                        in TrafficAreaNodeLookup,
                        in ParkingConfig);
                    return;
                }
                else
                {
                    float distanceToLocalTarget = math.distance(trafficPathComponent.DestinationWayPoint, carPosition);

                    destinationComponent.DistanceToWaypoint = distanceToLocalTarget;

                    var isRailMovement = TrafficRailMovementLookup.HasComponent(entity);

                    float checkDistanceToTargetRouteNode = !isRailMovement ? TrafficNavConfig.Config.Value.MinDistanceToTargetRouteNode : TrafficNavConfig.Config.Value.MinDistanceToTargetRailRouteNode;

                    bool forceSwitchNode = false;

                    var forward = math.mul(transform.Rotation, math.forward());

                    if (TrafficNavConfig.Config.Value.OutOfPathMethod != OutOfPathResolveMethod.Disabled)
                    {
                        float3 directionToNode = math.normalize(trafficPathComponent.DestinationWayPoint - carPosition).Flat();

                        var carDirection = trafficPathComponent.PathDirection == PathForwardType.Forward ? forward : -forward;
                        var inRange = distanceToLocalTarget > TrafficNavConfig.Config.Value.MinDistanceToOutOfPath && distanceToLocalTarget < TrafficNavConfig.Config.Value.MaxDistanceToOutOfPath;

                        if (inRange)
                        {
                            switch (TrafficNavConfig.Config.Value.OutOfPathMethod)
                            {
                                case OutOfPathResolveMethod.SwitchNode:
                                    {
                                        float dot = math.dot(directionToNode, carDirection);
                                        forceSwitchNode = dot < 0f;
                                        break;
                                    }
                                case OutOfPathResolveMethod.Backward:
                                    {
                                        if (trafficPathComponent.PathDirection == PathForwardType.Forward)
                                        {
                                            float dot = math.dot(directionToNode, forward);
                                            var isSidePoint = math.abs(dot) < 0.1f;

                                            if (isSidePoint)
                                            {
                                                var side = Vector3.SignedAngle(directionToNode, forward, Vector3.up) < 0 ? -1 : 1;
                                                var point = trafficPathComponent.DestinationWayPoint - math.normalize(trafficPathComponent.DestinationWayPoint - trafficPathComponent.PreviousDestination) * distanceToLocalTarget;

                                                TrafficCustomDestinationComponent customDestinationComponent = new TrafficCustomDestinationComponent()
                                                {
                                                    Destination = point
                                                };

                                                CommandBuffer.AddComponent(entityInQueryIndex, entity, customDestinationComponent);
                                                return;
                                            }
                                        }

                                        break;
                                    }
                                case OutOfPathResolveMethod.Cull:
                                    {
                                        if (!InViewOfCameraLookup.HasComponent(entity))
                                        {
                                            float dot = math.dot(directionToNode, carDirection);
                                            var cull = dot < 0f;

                                            if (cull)
                                            {
                                                PoolEntityUtils.DestroyEntity(ref CommandBuffer, entityInQueryIndex, entity);
                                                return;
                                            }
                                        }
                                        break;
                                    }
                            }
                        }
                    }

                    bool routeNodeIsAchieved = distanceToLocalTarget < checkDistanceToTargetRouteNode || forceSwitchNode;

                    if (routeNodeIsAchieved)
                    {
                        var pathNodes = Graph.GetRouteNodes(in pathData);

                        var currentLocalPathNodeIndex = trafficPathComponent.LocalPathNodeIndex;
                        var newLocalPathNodeIndex = math.clamp(currentLocalPathNodeIndex + 1, 0, pathNodes.Length - 1);

                        currentLocalPathNodeIndex = newLocalPathNodeIndex - 1;
                        var pathNode = pathNodes[currentLocalPathNodeIndex];

                        speedComponent.LaneLimit = pathNode.SpeedLimit;
                        trafficPathComponent.PathDirection = pathNode.ForwardNodeDirectionType;

                        if (TrafficGeneralSettingsReference.Config.Value.ChangeLaneSupport && pathData.HasOption(PathOptions.HasCustomNode))
                        {
                            var isAvailable = pathNode.IsAvailable(in trafficTypeComponent);

                            if (!isAvailable)
                            {
                                if (pathData.ParallelCount > 0)
                                {
                                    var parallelPathsIndexes = Graph.GetParallelPaths(in pathData);

                                    for (int j = 0; j < parallelPathsIndexes.Length; j++)
                                    {
                                        var parallelIndex = parallelPathsIndexes[j];
                                        ref readonly var parallelPathData = ref Graph.GetPathData(parallelIndex);

                                        isAvailable = parallelPathData.IsAvailable(in trafficTypeComponent);

                                        if (isAvailable)
                                        {
                                            var sourcePathEntity = RuntimePathDataRef.TryToGetSourceNode(parallelIndex);

                                            TrafficChangeLaneUtils.SetWaitForChangeLane(
                                                ref CommandBuffer,
                                                entityInQueryIndex,
                                                trafficPathComponent.CurrentGlobalPathIndex,
                                                parallelIndex,
                                                currentLocalPathNodeIndex,
                                                entity,
                                                ref trafficPathComponent,
                                                ref destinationComponent,
                                                ref trafficStateComponent,
                                                ref trafficIdleTagRW,
                                                in transform,
                                                sourcePathEntity,
                                                ref Graph,
                                                ref TrafficChangeLaneConfigReference,
                                                ref TrafficCommonSettingsConfigBlobReference,
                                                speedComponent.Value);

                                            return;
                                        }
                                    }
                                }
                            }
                        }

                        trafficPathComponent.LocalPathNodeIndex = newLocalPathNodeIndex;

                        Vector3 newTargetWaypoint = pathNodes[newLocalPathNodeIndex].Position;

                        if (!trafficPathComponent.DestinationWayPoint.Equals(float3.zero))
                        {
                            trafficPathComponent.PreviousDestination = trafficPathComponent.DestinationWayPoint;
                        }

                        trafficPathComponent.DestinationWayPoint = newTargetWaypoint;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool CheckIfNewTrafficNodeIsCloseEnough(ref TrafficDestinationConfigReference config, ref TrafficDestinationComponent destinationComponent, float distanceToNode)
        {
            if (distanceToNode < config.Config.Value.MinDistanceToNewLight && destinationComponent.CurrentNode != destinationComponent.DestinationNode)
            {
                destinationComponent.CurrentNode = destinationComponent.DestinationNode;
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CheckDistanceHowFarPreviousTrafficLight(ref TrafficDestinationConfigReference config, in ComponentLookup<LocalToWorld> nodePositions, ref TrafficDestinationComponent destinationComponent, in LocalTransform transform)
        {
            var currentNodeEntity = destinationComponent.CurrentNode;

            if (currentNodeEntity != Entity.Null && currentNodeEntity == destinationComponent.PreviousNode && nodePositions.HasComponent(currentNodeEntity))
            {
                float distanceToCurrentNode = math.distancesq(nodePositions[currentNodeEntity].Position, transform.Position);

                if (distanceToCurrentNode > config.Config.Value.MaxDistanceFromPreviousLightSQ)
                {
                    var dirToTarget = math.normalize(nodePositions[currentNodeEntity].Position - transform.Position);

                    var dot = math.dot(transform.Forward(), dirToTarget);

                    // The light is behind
                    if (dot < 0)
                    {
                        destinationComponent.CurrentNode = Entity.Null;
                    }
                }
            }
        }
    }
}