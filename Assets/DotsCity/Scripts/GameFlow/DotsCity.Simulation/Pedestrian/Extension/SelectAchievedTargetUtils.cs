using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Pedestrian.State;
using Spirit604.DotsCity.Simulation.Road;
using Spirit604.Gameplay.Road;
using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Transforms;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    public static class SelectAchievedTargetUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ProcessAchievedTarget(
            ref EntityCommandBuffer.ParallelWriter commandBuffer,
            in BufferLookup<NodeConnectionDataElement> nodeConnectionBufferLookup,
            in ComponentLookup<NodeSettingsComponent> nodeSettingsLookup,
            in ComponentLookup<NodeCapacityComponent> nodeCapacityLookup,
            in ComponentLookup<NodeLightSettingsComponent> nodeLightSettingsComponentLookup,
            in ComponentLookup<LightHandlerComponent> lightHandlerLookup,
            in ComponentLookup<LocalToWorld> worldTransformLookup,
            in DestinationConfigReference destinationConfigReference,
            in float timestamp,
            Entity pedestrianEntity,
            int entityInQueryIndex,
            ref DestinationComponent destinationComponent,
            ref NextStateComponent nextStateComponent,
            ref EnabledRefRW<HasTargetTag> hasTargetTagRW,
            in LocalToWorld worldTransform)
        {
            bool disableTarget = true;
            Entity achievedTargetEntity = destinationComponent.DestinationNode;

            if (!nodeSettingsLookup.HasComponent(achievedTargetEntity))
            {
                destinationComponent = destinationComponent.SwapBack();
                return;
            }

            NodeSettingsComponent nodeSettingsComponent = nodeSettingsLookup[achievedTargetEntity];

            switch (nodeSettingsComponent.NodeType)
            {
                case PedestrianNodeType.Default:
                    {
                        disableTarget = false;

                        ProcessDefaultNodeSystem.Process(
                            in nodeConnectionBufferLookup,
                            in nodeSettingsLookup,
                            in nodeCapacityLookup,
                            in nodeLightSettingsComponentLookup,
                            in lightHandlerLookup,
                            in worldTransformLookup,
                            in destinationConfigReference,
                            in timestamp,
                            pedestrianEntity,
                            ref destinationComponent,
                            ref nextStateComponent,
                            in worldTransform);

                        break;
                    }
                case PedestrianNodeType.House:
                    {
                        PoolEntityUtils.DestroyEntity(ref commandBuffer, entityInQueryIndex, pedestrianEntity);
                        break;
                    }
                case PedestrianNodeType.Sit:
                    {
                        commandBuffer.AddComponent<ProcessEnterSeatNodeTag>(entityInQueryIndex, pedestrianEntity);
                        break;
                    }
                case PedestrianNodeType.Idle:
                    {
                        commandBuffer.SetComponentEnabled<IdleTag>(entityInQueryIndex, pedestrianEntity, true);
                        commandBuffer.AddComponent(entityInQueryIndex, pedestrianEntity, new IdleTimeComponent()
                        {
                            IdleNode = achievedTargetEntity
                        });

                        break;
                    }
                case PedestrianNodeType.CarParking:
                    {
                        commandBuffer.AddComponent<ProcessEnterCarParkingNodeTag>(entityInQueryIndex, pedestrianEntity);
                        break;
                    }
                case PedestrianNodeType.TalkArea:
                    {
                        disableTarget = false;

                        ProcessDefaultNodeSystem.Process(
                            in nodeConnectionBufferLookup,
                            in nodeSettingsLookup,
                            in nodeCapacityLookup,
                            in nodeLightSettingsComponentLookup,
                            in lightHandlerLookup,
                            in worldTransformLookup,
                            in destinationConfigReference,
                            in timestamp,
                            pedestrianEntity,
                            ref destinationComponent,
                            ref nextStateComponent,
                            in worldTransform);

                        break;
                    }
                case PedestrianNodeType.TrafficPublicStopStation:
                    {
                        commandBuffer.AddComponent<ProcessEnterTrafficStationNodeTag>(entityInQueryIndex, pedestrianEntity);
                        break;
                    }
                case PedestrianNodeType.TrafficPublicEntry:
                    {
                        commandBuffer.AddComponent<ProcessEnterTrafficEntryNodeTag>(entityInQueryIndex, pedestrianEntity);
                        break;
                    }
            }

            if (disableTarget)
            {
                hasTargetTagRW.ValueRW = false;
            }
        }
    }
}