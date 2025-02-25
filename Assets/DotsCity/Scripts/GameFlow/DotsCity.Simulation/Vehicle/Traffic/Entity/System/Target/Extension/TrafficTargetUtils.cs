﻿using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Road;
using Spirit604.DotsCity.Simulation.TrafficArea;
using Spirit604.Gameplay.Road;
using System.Runtime.CompilerServices;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Traffic
{
    public static class TrafficTargetUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ProcessAchievedTarget(
            ref EntityCommandBuffer.ParallelWriter commandBuffer,
            Entity entity,
            int entityInQueryIndex,
            ref TrafficDestinationComponent destinationComponent,
            ref TrafficStateComponent trafficStateComponent,
            ref EnabledRefRW<TrafficEnteredTriggerNodeTag> trafficEnteredTriggerNodeTagRW,
            ref EnabledRefRW<TrafficEnteringTriggerNodeTag> trafficEnteringTriggerNodeTagRW,
            ref EnabledRefRW<TrafficSwitchTargetNodeRequestTag> trafficSwitchTargetNodeRequestTagRW,
            ref EnabledRefRW<TrafficIdleTag> trafficIdleTagRW,
            in ComponentLookup<TrafficNodeSettingsComponent> spawnPointSettingsLookup,
            in ComponentLookup<TrafficNodeCapacityComponent> trafficNodeCapacityLookup,
            in ComponentLookup<TrafficAreaNode> trafficAreaNodeLookup,
            in TrafficParkingConfigReference parkingConfig,
            bool lockAligment = false)
        {
            bool switchTarget = true;

            if (destinationComponent.PathConnectionType == PathConnectionType.TrafficNode)
            {
                var currentTrafficNodeEntity = destinationComponent.CurrentNode;

                if (currentTrafficNodeEntity != Entity.Null)
                {
                    var spawnpointSettingsComponent = spawnPointSettingsLookup[currentTrafficNodeEntity];
                    var capacityComponent = trafficNodeCapacityLookup[currentTrafficNodeEntity];
                    bool canLinkNode = capacityComponent.CanLinkNode(entity);

                    switch (spawnpointSettingsComponent.TrafficNodeType)
                    {
                        case TrafficNodeType.Parking:
                            {
                                if (canLinkNode)
                                {
                                    TrafficStateExtension.AddIdleState(ref trafficStateComponent, ref trafficIdleTagRW, TrafficIdleState.Parking);

                                    if (!parkingConfig.Config.Value.AligmentAtNode)
                                    {
                                        trafficEnteredTriggerNodeTagRW.ValueRW = true;
                                    }
                                    else
                                    {
                                        if (!capacityComponent.AlreadyLinked(entity))
                                        {
                                            trafficEnteringTriggerNodeTagRW.ValueRW = true;
                                        }

                                        commandBuffer.AddComponent<TrafficAccurateAligmentCustomMovementTag>(entityInQueryIndex, entity);
                                    }
                                }
                                else
                                {
                                    switchTarget = true;
                                }

                                break;
                            }
                        case TrafficNodeType.TrafficPublicStop:
                            {
                                if (canLinkNode)
                                {
                                    trafficEnteredTriggerNodeTagRW.ValueRW = true;

                                    if (!parkingConfig.Config.Value.AligmentAtNode || lockAligment)
                                    {
                                        trafficEnteredTriggerNodeTagRW.ValueRW = true;
                                    }
                                    else
                                    {
                                        if (!capacityComponent.AlreadyLinked(entity))
                                        {
                                            trafficEnteringTriggerNodeTagRW.ValueRW = true;
                                            commandBuffer.SetComponentEnabled<TrafficEnteringTriggerNodeTag>(entityInQueryIndex, entity, true);
                                        }

                                        commandBuffer.AddComponent<TrafficAccurateAligmentCustomMovementTag>(entityInQueryIndex, entity);
                                    }
                                }
                                else
                                {
                                    switchTarget = true;
                                }

                                break;
                            }
                        case TrafficNodeType.DestroyVehicle:
                            {
                                switchTarget = false;
                                PoolEntityUtils.DestroyEntity(ref commandBuffer, entityInQueryIndex, entity);
                                break;
                            }
                        case TrafficNodeType.TrafficArea:
                            {
                                break;
                            }
                        case TrafficNodeType.Idle:
                            {
                                commandBuffer.AddComponent<TrafficIdleNodeProcessComponent>(entityInQueryIndex, entity);
                                break;
                            }
                    }

                    if (trafficAreaNodeLookup.HasComponent(currentTrafficNodeEntity))
                    {
                        commandBuffer.SetComponent(entityInQueryIndex, currentTrafficNodeEntity, new TrafficAreaEntryNodeComponent()
                        {
                            TrafficEntity = entity
                        });

                        commandBuffer.SetComponentEnabled<TrafficAreaProcessEnteredNodeTag>(entityInQueryIndex, currentTrafficNodeEntity, true);
                    }
                }
            }

            if (switchTarget)
            {
                trafficSwitchTargetNodeRequestTagRW.ValueRW = true;
            }
        }
    }
}