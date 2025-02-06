using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Npc;
using Spirit604.DotsCity.Simulation.Pedestrian.State;
using Unity.Burst;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [UpdateAfter(typeof(HealthWithRagdollSystem))]
    [UpdateInGroup(typeof(PedestrianSimulationGroup))]
    [BurstCompile]
    public partial struct PoolNoRagdollSystem : ISystem
    {
        private EntityQuery updateGroup;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            updateGroup = SystemAPI.QueryBuilder()
                .WithDisabled<PooledEventTag, HasSkinTag>()
                .WithAll<RagdollActivateEventTag, StateComponent>()
                .Build();

            state.RequireForUpdate(updateGroup);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var pedestrianCleanNoSkinJob = new CleanNoSkinJob()
            {
            };

            pedestrianCleanNoSkinJob.Schedule();
        }

        [WithDisabled(typeof(PooledEventTag), typeof(HasSkinTag))]
        [WithAll(typeof(RagdollActivateEventTag))]
        [BurstCompile]
        public partial struct CleanNoSkinJob : IJobEntity
        {
            void Execute(
                EnabledRefRW<PooledEventTag> pooledEventTagRW)
            {
                PoolEntityUtils.DestroyEntity(ref pooledEventTagRW);
            }
        }
    }
}