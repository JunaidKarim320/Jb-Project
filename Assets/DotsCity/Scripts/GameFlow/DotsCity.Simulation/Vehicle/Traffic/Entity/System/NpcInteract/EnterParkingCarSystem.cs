using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Car;
using Spirit604.DotsCity.Simulation.Sound.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Spirit604.DotsCity.Simulation.Traffic
{
    [UpdateInGroup(typeof(LateInitGroup))]
    [BurstCompile]
    public partial struct EnterParkingCarSystem : ISystem
    {
        private EntityQuery carQuery;
        private EntityQuery soundPrefabQuery;

        void ISystem.OnCreate(ref SystemState state)
        {
            soundPrefabQuery = SoundExtension.GetSoundQuery(state.EntityManager);

            carQuery = SystemAPI.QueryBuilder()
                .WithAllRW<TrafficStateComponent>()
                .WithPresentRW<TrafficIdleTag>()
                .WithAll<ParkingDriverRequestTag, CarModelComponent, LocalTransform>()
                .Build();

            state.RequireForUpdate(carQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var hasIgnition = false;

            if (SystemAPI.HasSingleton<CarIgnitionConfigReference>())
            {
                hasIgnition = SystemAPI.GetSingleton<CarIgnitionConfigReference>().Config.Value.HasIgnition;
            }

            var enterCarJob = new EnterCarJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
                InViewOfCameraLookup = SystemAPI.GetComponentLookup<InViewOfCameraTag>(true),
                CarSharedDataConfigReference = SystemAPI.GetSingleton<CarSharedDataConfigReference>(),
                HasIgnition = hasIgnition,
                SoundPrefabEntity = soundPrefabQuery.GetSingletonEntity(),
            };

            enterCarJob.Schedule(carQuery);
        }

        [WithAll(typeof(ParkingDriverRequestTag))]
        [BurstCompile]
        public partial struct EnterCarJob : IJobEntity
        {
            public EntityCommandBuffer CommandBuffer;

            [ReadOnly]
            public ComponentLookup<InViewOfCameraTag> InViewOfCameraLookup;

            [ReadOnly]
            public CarSharedDataConfigReference CarSharedDataConfigReference;

            [ReadOnly]
            public bool HasIgnition;

            [ReadOnly]
            public Entity SoundPrefabEntity;

            void Execute(
                Entity entity,
                ref TrafficStateComponent trafficStateComponent,
                EnabledRefRW<TrafficIdleTag> trafficIdleTagRW,
                in CarModelComponent carModelComponent,
                in LocalTransform transform)
            {
                var inViewOfCamera = InViewOfCameraLookup.IsComponentEnabled(entity);
                var ignite = HasIgnition && inViewOfCamera;

                ref var soundConfig = ref CarSharedDataConfigReference.Config;

                InteractCarUtils.EnterCar(ref CommandBuffer, ref soundConfig, SoundPrefabEntity, entity, inViewOfCamera, ignite, carModelComponent.Value, transform.Position);
                TrafficStateExtension.RemoveIdleState(ref trafficStateComponent, ref trafficIdleTagRW, TrafficIdleState.Parking);

                CommandBuffer.RemoveComponent<TrafficNodeLinkedComponent>(entity);
                CommandBuffer.RemoveComponent<ParkingDriverRequestTag>(entity);
            }
        }
    }
}