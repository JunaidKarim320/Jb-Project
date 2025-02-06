using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [UpdateInGroup(typeof(PedestrianSimulationGroup))]
    [BurstCompile]
    public partial struct TriggerImpactSystem : ISystem
    {
        private EntityQuery updateQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            updateQuery = SystemAPI.QueryBuilder()
                .WithAll<HasImpactTriggerTag>()
                .Build();

            state.RequireForUpdate(updateQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var impactTriggerJob = new ImpactTriggerJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
                CurrentTime = (float)SystemAPI.Time.ElapsedTime,
            };

            impactTriggerJob.Schedule();
        }

        [WithAll(typeof(HasImpactTriggerTag))]
        [BurstCompile]
        public partial struct ImpactTriggerJob : IJobEntity
        {
            public EntityCommandBuffer CommandBuffer;

            [ReadOnly]
            public float CurrentTime;

            void Execute(
                Entity entity,
                ref ImpactTriggerData pedestrianImpactTriggerData)
            {
                if (pedestrianImpactTriggerData.EndTime == 0)
                {
                    pedestrianImpactTriggerData.EndTime = pedestrianImpactTriggerData.Duration + CurrentTime;
                }

                if (CurrentTime >= pedestrianImpactTriggerData.EndTime)
                {
                    CommandBuffer.RemoveComponent<HasImpactTriggerTag>(entity);
                    CommandBuffer.RemoveComponent<ImpactTriggerData>(entity);
                }
            }
        }
    }
}