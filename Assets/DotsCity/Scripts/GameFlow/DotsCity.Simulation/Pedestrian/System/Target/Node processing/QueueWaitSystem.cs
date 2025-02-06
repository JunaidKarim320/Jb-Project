using Spirit604.DotsCity.Simulation.Car;
using Spirit604.DotsCity.Simulation.Pedestrian.State;
using Spirit604.DotsCity.Simulation.Road;
using Spirit604.DotsCity.Simulation.Traffic;
using Spirit604.DotsCity.Simulation.TrafficPublic;
using Spirit604.Extensions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [UpdateInGroup(typeof(LateInitGroup))]
    [BurstCompile]
    public partial struct QueueWaitSystem : ISystem
    {
        private const float CustomAchieveEntryPointDistance = 0.5f;

        private EntityQuery updateQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            updateQuery = SystemAPI.QueryBuilder()
                .WithAll<NodeProcessWaitQueueTag>()
                .Build();

            state.RequireForUpdate(updateQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var queueWaitJob = new QueueWaitJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
                NodeCapacityLookup = SystemAPI.GetComponentLookup<NodeCapacityComponent>(false),
                TrafficNodeCapacityLookup = SystemAPI.GetComponentLookup<TrafficNodeCapacityComponent>(true),
                TrafficPublicExitCompleteLookup = SystemAPI.GetComponentLookup<TrafficPublicExitCompleteTag>(true),
                CarCapacityLookup = SystemAPI.GetComponentLookup<CarCapacityComponent>(true),
                TrafficPublicExitSettingsLookup = SystemAPI.GetComponentLookup<TrafficPublicExitSettingsComponent>(true),
                TrafficWagonElementLookup = SystemAPI.GetBufferLookup<TrafficWagonElement>(true),
                VehicleEntryLookup = SystemAPI.GetBufferLookup<VehicleEntryElement>(true),
                DestinationLookup = SystemAPI.GetComponentLookup<DestinationComponent>(true),
                WorldTransformLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true),
                Timestamp = (float)SystemAPI.Time.ElapsedTime,
            };

            queueWaitJob.Schedule();
        }

        [WithAll(typeof(NodeProcessWaitQueueTag))]
        [BurstCompile]
        public partial struct QueueWaitJob : IJobEntity
        {
            public EntityCommandBuffer CommandBuffer;

            public ComponentLookup<NodeCapacityComponent> NodeCapacityLookup;

            [ReadOnly]
            public ComponentLookup<TrafficNodeCapacityComponent> TrafficNodeCapacityLookup;

            [ReadOnly]
            public ComponentLookup<TrafficPublicExitCompleteTag> TrafficPublicExitCompleteLookup;

            [ReadOnly]
            public ComponentLookup<CarCapacityComponent> CarCapacityLookup;

            [ReadOnly]
            public ComponentLookup<TrafficPublicExitSettingsComponent> TrafficPublicExitSettingsLookup;

            [ReadOnly]
            public BufferLookup<TrafficWagonElement> TrafficWagonElementLookup;

            [ReadOnly]
            public BufferLookup<VehicleEntryElement> VehicleEntryLookup;

            [ReadOnly]
            public ComponentLookup<DestinationComponent> DestinationLookup;

            [ReadOnly]
            public ComponentLookup<LocalToWorld> WorldTransformLookup;

            [ReadOnly]
            public float Timestamp;

            void Execute(
                Entity entity,
                DynamicBuffer<WaitQueueElement> waitQueue,
                ref WaitQueueComponent waitQueueComponent,
                in NodeLinkedTrafficNodeComponent linkedNodeComponent)
            {
                var trafficNodeCapacityComponent = TrafficNodeCapacityLookup[linkedNodeComponent.LinkedEntity];

                if (!trafficNodeCapacityComponent.HasCar())
                    return;

                var carEntity = trafficNodeCapacityComponent.CarEntity;

                if (TrafficWagonElementLookup.HasBuffer(carEntity))
                {
                    var carPos = WorldTransformLookup[carEntity].Position;
                    var nodePos = WorldTransformLookup[entity].Position;
                    float maxDistance = math.distancesq(carPos, nodePos);

                    var buffer = TrafficWagonElementLookup[carEntity];

                    for (int i = 0; i < buffer.Length; i++)
                    {
                        var wagonPos = WorldTransformLookup[buffer[i].Entity].Position;

                        var distance = math.distancesq(wagonPos, nodePos);

                        if (distance < maxDistance)
                        {
                            var currentCarCapacity = CarCapacityLookup[buffer[i].Entity];

                            if (currentCarCapacity.AvailableCapacity > 0)
                            {
                                maxDistance = distance;
                                carEntity = buffer[i].Entity;
                            }
                        }
                    }
                }

                bool exitting = !TrafficPublicExitCompleteLookup.HasComponent(carEntity) || !TrafficPublicExitCompleteLookup.IsComponentEnabled(carEntity);

                if (exitting)
                    return;

                var carCapacity = CarCapacityLookup[carEntity];
                var trafficPublicExitSettingsComponent = TrafficPublicExitSettingsLookup[carEntity];

                var shouldActivate = Timestamp - waitQueueComponent.LastActiveTimeStamp >= 0;

                if (shouldActivate)
                {
                    var duration = UnityMathematicsExtension.GetRandomValue(trafficPublicExitSettingsComponent.EnterExitDelayDuration, Timestamp, entity.Index);
                    waitQueueComponent.LastActiveTimeStamp = Timestamp + duration;

                    if (carCapacity.AvailableCapacity > 0 && VehicleEntryLookup.HasBuffer(carEntity) && VehicleEntryLookup[carEntity].Length > 0)
                    {
                        for (int i = 0; i < waitQueue.Length; i++)
                        {
                            if (!waitQueue[i].Activated)
                            {
                                waitQueueComponent.ActivatedCount++;

                                var pedestrianWaitQueueElement = waitQueue[i];
                                pedestrianWaitQueueElement.Activated = true;
                                waitQueue[i] = pedestrianWaitQueueElement;

                                var pedestrianEntity = waitQueue[i].PedestrianEntity;

                                if (!DestinationLookup.HasComponent(pedestrianEntity))
                                    break;

                                CommandBuffer.SetComponentEnabled<HasTargetTag>(pedestrianEntity, true);

                                var previousPedestrianDestinationComponent = DestinationLookup[pedestrianEntity];
                                var nodeEntity = previousPedestrianDestinationComponent.DestinationNode;

                                var entries = VehicleEntryLookup[carEntity];
                                var randomEntry = entries[pedestrianEntity.Index % entries.Length].EntryPointEntity;
                                var destination = WorldTransformLookup[randomEntry].Position;

                                CommandBuffer.SetComponent(pedestrianEntity, new DestinationComponent()
                                {
                                    Value = destination,
                                    PreviousDestination = previousPedestrianDestinationComponent.Value,
                                    PreviuosDestinationNode = nodeEntity,
                                    DestinationNode = randomEntry,
                                    PreviousLightEntity = Entity.Null,
                                    DestinationLightEntity = Entity.Null,
                                    CustomAchieveDistance = CustomAchieveEntryPointDistance,
                                    CustomAchieveDistanceSQ = CustomAchieveEntryPointDistance * CustomAchieveEntryPointDistance
                                });

                                CommandBuffer.SetComponent(pedestrianEntity, new NextStateComponent(ActionState.MovingToNextTargetPoint));
                                CommandBuffer.RemoveComponent<IdleTimeComponent>(pedestrianEntity);

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
