using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Car;
using Spirit604.DotsCity.Simulation.Level.Streaming;
using Spirit604.DotsCity.Simulation.Road;
using Spirit604.DotsCity.Simulation.Traffic;
using Spirit604.DotsCity.Simulation.TrafficArea;
using Spirit604.Extensions;
using Spirit604.Gameplay.Road;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Train
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
    [BurstCompile]
    public partial struct TrafficTrainTargetSystem : ISystem
    {
        private EntityQuery updateQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            updateQuery = SystemAPI.QueryBuilder()
                .WithNone<TrafficCustomDestinationComponent, TrafficNoTargetTag>()
                .WithDisabled<TrafficChangingLaneEventTag>()
                .WithDisabledRW<TrafficSwitchTargetNodeRequestTag>()
                .WithAllRW<SpeedComponent, TrafficDestinationComponent>()
                .WithAllRW<TrafficPathComponent, TrafficStateComponent>()
                .WithPresentRW<TrafficEnteredTriggerNodeTag, TrafficEnteringTriggerNodeTag>()
                .WithPresentRW<TrafficIdleTag>()
                .WithAll<TrafficTag, HasDriverTag, TrafficTypeComponent, LocalTransform, TrainTag, TrainComponent>()
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
                DeltaTime = SystemAPI.Time.DeltaTime
            };

            targetJob.Schedule(updateQuery);
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

            [ReadOnly]
            public float DeltaTime;

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
                in TrainComponent trainComponent,
                in TrafficTypeComponent trafficTypeComponent,
                in LocalTransform transform)
            {
                float3 carPosition = transform.Position;
                float3 currentTargetPosition = destinationComponent.Destination;
                ref readonly var pathData = ref Graph.GetPathData(trafficPathComponent.CurrentGlobalPathIndex);

                var dstNode = destinationComponent.DestinationNode;
                var hasDstNode = NodeSettingsLookup.HasComponent(dstNode);

                float distanceToTarget = math.distance(currentTargetPosition, carPosition);
                TrafficTargetSystem.CheckDistanceHowFarPreviousTrafficLight(ref TrafficNavConfig, in WorldTransformLookup, ref destinationComponent, in transform);

                if (destinationComponent.PathConnectionType != PathConnectionType.PathPoint)
                {
                    var endPosition = Graph.GetEndPosition(in pathData);
                    float distanceToTargetNode = math.distance(endPosition, carPosition);

                    destinationComponent.DistanceToEndOfPath = distanceToTargetNode;

                    var nextNodeRequest = TrafficTargetSystem.CheckIfNewTrafficNodeIsCloseEnough(ref TrafficNavConfig, ref destinationComponent, distanceToTargetNode);

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

                bool switchToNextTarget = false;

                float distanceToLocalTarget = math.distance(trafficPathComponent.DestinationWayPoint, carPosition);

                destinationComponent.DistanceToWaypoint = distanceToLocalTarget;

                var isRailMovement = TrafficRailMovementLookup.HasComponent(entity);

                float checkDistanceToTargetRouteNode = !isRailMovement ? TrafficNavConfig.Config.Value.MinDistanceToTargetRouteNode : TrafficNavConfig.Config.Value.MinDistanceToTargetRailRouteNode;

                bool forceSwitchNode = false;

                var forward = math.mul(transform.Rotation, math.forward());

                if (destinationComponent.DistanceToWaypoint < 1)
                {
                    float3 directionToNode = math.normalize(trafficPathComponent.DestinationWayPoint - carPosition).Flat();

                    var carDirection = trafficPathComponent.PathDirection == PathForwardType.Forward ? forward : -forward;
                    var inRange = distanceToLocalTarget > TrafficNavConfig.Config.Value.MinDistanceToOutOfPath && distanceToLocalTarget < TrafficNavConfig.Config.Value.MaxDistanceToOutOfPath;

                    float dot = math.dot(directionToNode, carDirection);

                    forceSwitchNode = dot < 0f;
                }

                var deltaDistance = speedComponent.Value * DeltaTime;
                bool routeNodeIsAchieved = destinationComponent.DistanceToWaypoint < deltaDistance || forceSwitchNode;
                //bool routeNodeIsAchieved = forceSwitchNode;

                if (routeNodeIsAchieved)
                {
                    var pathNodes = Graph.GetRouteNodes(in pathData);

                    var currentLocalPathNodeIndex = trafficPathComponent.LocalPathNodeIndex;
                    var newLocalPathNodeIndex = currentLocalPathNodeIndex + 1;

                    if (newLocalPathNodeIndex >= pathNodes.Length)
                    {
                        switchToNextTarget = true;
                    }
                    else
                    {
                        currentLocalPathNodeIndex = newLocalPathNodeIndex - 1;
                        var pathNode = pathNodes[currentLocalPathNodeIndex];

                        speedComponent.LaneLimit = pathNode.SpeedLimit;
                        trafficPathComponent.PathDirection = pathNode.ForwardNodeDirectionType;

                        trafficPathComponent.LocalPathNodeIndex = newLocalPathNodeIndex;

                        Vector3 newTargetWaypoint = pathNodes[newLocalPathNodeIndex].Position;

                        if (!trafficPathComponent.DestinationWayPoint.Equals(float3.zero))
                        {
                            trafficPathComponent.PreviousDestination = trafficPathComponent.DestinationWayPoint;
                        }

                        trafficPathComponent.DestinationWayPoint = newTargetWaypoint;
                        destinationComponent.DistanceToWaypoint = math.distance(trafficPathComponent.DestinationWayPoint, carPosition);
                    }
                }

                if (switchToNextTarget)
                {
                    if (!hasDstNode)
                    {
                        // Destination node unloaded due to road streaming
                        TrafficNoTargetUtils.AddNoTarget(ref CommandBuffer, entity, entityInQueryIndex, ref trafficStateComponent, ref trafficIdleTagRW, in TrafficDestinationConfigReference);
                        return;
                    }

                    if (!trainComponent.IsParent)
                    {
                        trafficSwitchTargetNodeRequestTagRW.ValueRW = true;
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
                        in ParkingConfig,
                        true);
                }
            }
        }
    }
}