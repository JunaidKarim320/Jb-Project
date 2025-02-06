using Unity.Burst;
using Unity.Entities;

namespace Spirit604.DotsCity.Core
{

    [UpdateInGroup(typeof(InitGroup), OrderFirst = true)]
    [BurstCompile]
    public partial struct PoolEntitySystem : ISystem
    {
        private EntityQuery destroyQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            destroyQuery = SystemAPI.QueryBuilder()
                .WithAll<WorldEntitySharedType, PooledEventTag, PoolableTag>()
                .Build();

            destroyQuery.SetSharedComponentFilter(new WorldEntitySharedType(EntityWorldType.PureEntity));

            state.RequireForUpdate(destroyQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var commandBuffer = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            var poolJob = new DestroyEntityJob()
            {
                commandBuffer = commandBuffer
            };

            poolJob.Schedule(destroyQuery);
        }
    }
}