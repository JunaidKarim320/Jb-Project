using Spirit604.DotsCity.Core;
using Unity.Burst;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [UpdateAfter(typeof(DisableSpawnAreaSystem))]
    [UpdateInGroup(typeof(LateInitGroup))]
    [BurstCompile]
    public partial struct EnableSpawnAreaSystem : ISystem
    {
        private SystemHandle disableSpawnAreaSystem;
        private EntityQuery updateQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            disableSpawnAreaSystem = state.WorldUnmanaged.GetExistingUnmanagedSystem<DisableSpawnAreaSystem>();

            updateQuery = SystemAPI.QueryBuilder()
                .WithNone<NodeAreaSpawnedTag, CulledEventTag>()
                .WithDisabledRW<NodeAreaSpawnRequestedTag>()
                .WithAll<SpawnAreaComponent>()
                .WithAny<InPermittedRangeTag, InViewOfCameraTag>()
                .Build();

            state.RequireForUpdate(updateQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            ref var disableSpawnAreaSystemRef = ref state.WorldUnmanaged.ResolveSystemStateRef(disableSpawnAreaSystem);

            var enableSpawnAreaJob = new EnableSpawnAreaJob()
            {
            };

            state.Dependency = enableSpawnAreaJob.Schedule(disableSpawnAreaSystemRef.Dependency);
        }

        [WithNone(typeof(NodeAreaSpawnedTag), typeof(CulledEventTag))]
        [WithDisabled(typeof(NodeAreaSpawnRequestedTag))]
        [WithAny(typeof(InPermittedRangeTag), typeof(InViewOfCameraTag))]
        [BurstCompile]
        public partial struct EnableSpawnAreaJob : IJobEntity
        {
            void Execute(
                EnabledRefRW<NodeAreaSpawnRequestedTag> nodeAreaSpawnRequestedTagRW)
            {
                nodeAreaSpawnRequestedTagRW.ValueRW = true;
            }
        }
    }
}