using Unity.Burst;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.TrafficArea
{
    [UpdateInGroup(typeof(TrafficAreaSimulationGroup))]
    [BurstCompile]
    public partial struct TrafficAreaObserveEnableSystem : ISystem
    {
        private EntityQuery updateQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            updateQuery = SystemAPI.QueryBuilder()
                .WithNone<TrafficAreaCarObserverEnabledTag>()
                .WithAny<TrafficAreaProcessingEnterQueueTag, TrafficAreaProcessingExitQueueTag>()
                .WithAll<TrafficAreaComponent>()
                .Build();

            state.RequireForUpdate(updateQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var enableObserveJob = new EnableObserveJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
            };

            enableObserveJob.Schedule();
        }

        [WithNone(typeof(TrafficAreaCarObserverEnabledTag))]
        [WithAny(typeof(TrafficAreaProcessingEnterQueueTag), typeof(TrafficAreaProcessingExitQueueTag))]
        [BurstCompile]
        public partial struct EnableObserveJob : IJobEntity
        {
            public EntityCommandBuffer CommandBuffer;

            void Execute(
                Entity entity,
                in TrafficAreaTag trafficAreaComponent)
            {
                CommandBuffer.SetComponentEnabled<TrafficAreaCarObserverEnabledTag>(entity, true);
            }
        }
    }
}