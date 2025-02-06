using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Car;
using Spirit604.DotsCity.Simulation.Pedestrian.State;
using Spirit604.DotsCity.Simulation.Sound.Pedestrian;
using Spirit604.DotsCity.Simulation.Sound.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [UpdateInGroup(typeof(LateInitGroup))]
    [BurstCompile]
    public partial struct ProcessTrafficEntryNodeSystem : ISystem
    {
        // If the vehicle has departed from the stop station
        private const float MaxDiffDistanceSQ = 0.0625f; // 0.25f * 0.25f;

        private EntityQuery soundPrefabQuery;
        private EntityQuery npcQuery;

        void ISystem.OnCreate(ref SystemState state)
        {
            soundPrefabQuery = SoundExtension.GetSoundQuery(state.EntityManager);

            npcQuery = SystemAPI.QueryBuilder()
                .WithAll<ProcessEnterTrafficEntryNodeTag>()
                .Build();

            state.RequireForUpdate(npcQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var enteredTrafficEntryNodeJob = new EnteredTrafficEntryNodeJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
                CarCapacityLookup = SystemAPI.GetComponentLookup<CarCapacityComponent>(false),
                VehicleLinkLookup = SystemAPI.GetComponentLookup<VehicleLinkComponent>(true),
                WorldTransformLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true),
                SoundPrefabEntity = soundPrefabQuery.GetSingletonEntity(),
                SoundConfigReference = SystemAPI.GetSingleton<SoundConfigReference>()
            };

            enteredTrafficEntryNodeJob.Schedule();
        }

        [WithDisabled(typeof(PooledEventTag))]
        [WithAll(typeof(ProcessEnterTrafficEntryNodeTag))]
        [BurstCompile]
        public partial struct EnteredTrafficEntryNodeJob : IJobEntity
        {
            public EntityCommandBuffer CommandBuffer;

            public ComponentLookup<CarCapacityComponent> CarCapacityLookup;

            [ReadOnly]
            public ComponentLookup<VehicleLinkComponent> VehicleLinkLookup;

            [ReadOnly]
            public ComponentLookup<LocalToWorld> WorldTransformLookup;

            [ReadOnly]
            public Entity SoundPrefabEntity;

            [ReadOnly]
            public SoundConfigReference SoundConfigReference;

            void Execute(
                Entity pedestrianEntity,
                ref DestinationComponent destinationComponent,
                EnabledRefRW<PooledEventTag> pooledEventTagRW,
                in LocalTransform transform)
            {
                bool carEntered = false;

                if (VehicleLinkLookup.HasComponent(destinationComponent.DestinationNode))
                {
                    var carEntity = VehicleLinkLookup[destinationComponent.DestinationNode].LinkedVehicle;

                    if (CarCapacityLookup.HasComponent(carEntity))
                    {
                        var currentPosition = WorldTransformLookup[destinationComponent.DestinationNode].Position;
                        var destinationPosition = destinationComponent.Value;

                        var distance = math.distancesq(currentPosition, destinationPosition);

                        if (distance < MaxDiffDistanceSQ)
                        {
                            var carCapacity = CarCapacityLookup[carEntity];

                            if (carCapacity.AvailableCapacity > 0)
                            {
                                carCapacity.AvailableCapacity -= 1;
                                carEntered = true;
                            }

                            CarCapacityLookup[carEntity] = carCapacity;
                        }
                    }
                }

                if (carEntered)
                {
                    var soundId = SoundConfigReference.Config.Value.EnterTramSoundId;
                    CommandBuffer.CreateSoundEntity(SoundPrefabEntity, soundId, transform.Position);

                    PoolEntityUtils.DestroyEntity(ref pooledEventTagRW);
                }
                else
                {
                    destinationComponent = destinationComponent.SwapBack();

                    CommandBuffer.SetComponentEnabled<HasTargetTag>(pedestrianEntity, true);
                    CommandBuffer.RemoveComponent<ProcessEnterTrafficEntryNodeTag>(pedestrianEntity);
                }
            }
        }
    }
}
