using Unity.Burst;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Traffic
{
    [UpdateAfter(typeof(TrafficDisableCustomMovementStateSystem))]
    [UpdateInGroup(typeof(LateInitGroup))]
    [BurstCompile]
    public partial struct TrafficEnableCustomMovementStateSystem : ISystem
    {
        private EntityQuery updateGroup;

        void ISystem.OnCreate(ref SystemState state)
        {
            updateGroup = SystemAPI.QueryBuilder()
                .WithAny<TrafficRailMovementTag, TrafficAccurateAligmentCustomMovementTag>()
                .WithDisabledRW<TrafficCustomMovementTag>()
                .Build();

            state.RequireForUpdate(updateGroup);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var enableCustomStateJob = new EnableCustomStateJob()
            {
            };

            enableCustomStateJob.Schedule(updateGroup);
        }

        [BurstCompile]
        public partial struct EnableCustomStateJob : IJobEntity
        {
            void Execute(
               EnabledRefRW<TrafficCustomMovementTag> trafficCustomMovementTagRW)
            {
                trafficCustomMovementTagRW.ValueRW = true;
            }
        }
    }
}