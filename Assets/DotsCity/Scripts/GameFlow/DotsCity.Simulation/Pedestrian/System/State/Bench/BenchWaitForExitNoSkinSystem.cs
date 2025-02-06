using Unity.Burst;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Pedestrian.State
{
    [UpdateInGroup(typeof(PedestrianSimulationGroup))]
    [BurstCompile]
    public partial struct BenchWaitForExitNoSkinSystem : ISystem
    {
        private SystemHandle unloadGPUSkinSystem;
        private EntityQuery updateGroup;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            unloadGPUSkinSystem = state.WorldUnmanaged.GetExistingUnmanagedSystem<UnloadGPUSkinSystem>();

            updateGroup = SystemAPI.QueryBuilder()
                .WithNone<HasSkinTag>()
                .WithAll<BenchWaitForExitTag>()
                .Build();

            state.RequireForUpdate(updateGroup);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            ref var unloadGPUSkinSystemRef = ref state.WorldUnmanaged.ResolveSystemStateRef(unloadGPUSkinSystem);

            unloadGPUSkinSystemRef.Dependency.Complete();

            var noSkinWaitForExitJob = new NoSkinWaitForExitJob()
            {
            };

            state.Dependency = noSkinWaitForExitJob.Schedule(state.Dependency);
        }

        [WithNone(typeof(HasSkinTag))]
        [WithAll(typeof(BenchWaitForExitTag))]
        [BurstCompile]
        public partial struct NoSkinWaitForExitJob : IJobEntity
        {
            void Execute(
                ref SeatSlotLinkedComponent seatSlotLinkedComponent,
                EnabledRefRW<BenchWaitForExitTag> benchWaitForExitTagRW)
            {
                seatSlotLinkedComponent.Exited = true;
                benchWaitForExitTagRW.ValueRW = false;
            }
        }
    }
}
