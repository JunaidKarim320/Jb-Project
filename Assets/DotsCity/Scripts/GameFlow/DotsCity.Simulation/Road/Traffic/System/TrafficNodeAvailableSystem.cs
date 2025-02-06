using Unity.Burst;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Road
{
    [UpdateAfter(typeof(TrafficNodeCalculateOverlapSystem))]
    [UpdateInGroup(typeof(HashMapGroup))]
    [BurstCompile]
    public partial struct TrafficNodeAvailableSystem : ISystem
    {
        private SystemHandle trafficNodeCalculateOverlapSystem;
        private EntityQuery updateGroup;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            trafficNodeCalculateOverlapSystem = state.WorldUnmanaged.GetExistingUnmanagedSystem<TrafficNodeCalculateOverlapSystem>();

            updateGroup = SystemAPI.QueryBuilder()
                .WithPresentRW<TrafficNodeAvailableTag>()
                .WithAll<TrafficNodeAvailableComponent, TrafficNodeCapacityComponent>()
                .Build();

            state.RequireForUpdate(updateGroup);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            state.Dependency.Complete();

            ref var trafficNodeCalculateOverlapSystemRef = ref state.WorldUnmanaged.ResolveSystemStateRef(trafficNodeCalculateOverlapSystem);

            var availableNodeJob = new AvailableNodeJob()
            {
            };

            state.Dependency = availableNodeJob.ScheduleParallel(updateGroup, trafficNodeCalculateOverlapSystemRef.Dependency);
        }

        [BurstCompile]
        private partial struct AvailableNodeJob : IJobEntity
        {
            void Execute(
                EnabledRefRW<TrafficNodeAvailableTag> trafficNodeAvailableRW,
                in TrafficNodeAvailableComponent trafficNodeAvailableComponent,
                in TrafficNodeCapacityComponent trafficNodeCapacityComponent)
            {
                var currentAvailableState = trafficNodeAvailableComponent.IsAvailable && trafficNodeCapacityComponent.HasSlots();

                if (trafficNodeAvailableRW.ValueRW != currentAvailableState)
                {
                    trafficNodeAvailableRW.ValueRW = currentAvailableState;
                }
            }
        }
    }
}