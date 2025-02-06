using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Spirit604.DotsCity.Core
{
    [UpdateInGroup(typeof(InitGroup))]
    [BurstCompile]
    public partial struct CleanStaticPhysicsSystem : ISystem
    {
        private EntityQuery cullQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            cullQuery = SystemAPI.QueryBuilder()
                .WithAll<CullPhysicsTag, Static, CullStateComponent>()
                .Build();

            state.RequireForUpdate(cullQuery);
            state.Enabled = false;
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var cleanStaticPhysicsJob = new CleanStaticPhysicsJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
            };

            state.Dependency = cleanStaticPhysicsJob.Schedule(cullQuery, state.Dependency);
        }

        [WithAll(typeof(CullPhysicsTag), typeof(Static), typeof(CullStateComponent))]
        [BurstCompile]
        public partial struct CleanStaticPhysicsJob : IJobEntity
        {
            public EntityCommandBuffer CommandBuffer;

            void Execute(Entity entity)
            {
                CommandBuffer.SetComponentEnabled<CullStateComponent>(entity, false);
            }
        }
    }
}