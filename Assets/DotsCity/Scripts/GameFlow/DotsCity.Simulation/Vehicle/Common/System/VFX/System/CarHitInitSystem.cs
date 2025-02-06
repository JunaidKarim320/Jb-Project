using Unity.Burst;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Car
{
    [UpdateInGroup(typeof(CarSimulationGroup))]
    [BurstCompile]
    public partial struct CarHitInitSystem : ISystem
    {
        private EntityQuery updateQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            updateQuery = SystemAPI.QueryBuilder()
                .WithAll<HitReactionInitComponent>()
                .Build();

            state.RequireForUpdate(updateQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var hitInitJob = new HitInitJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
                CarHitReactionLookup = SystemAPI.GetComponentLookup<CarHitReactionData>(false),
            };

            hitInitJob.Schedule();
        }

        [BurstCompile]
        public partial struct HitInitJob : IJobEntity
        {
            public EntityCommandBuffer CommandBuffer;

            public ComponentLookup<CarHitReactionData> CarHitReactionLookup;

            void Execute(
                Entity entity,
                in HitReactionInitComponent hitReactionInitComponent)
            {
                CommandBuffer.SetComponentEnabled<HitReactionInitComponent>(entity, false);

                var carHitReaction = CarHitReactionLookup[hitReactionInitComponent.VehicleEntity];

                carHitReaction.HitMeshEntity = entity;

                CarHitReactionLookup[hitReactionInitComponent.VehicleEntity] = carHitReaction;
            }
        }
    }
}