using Unity.Burst;
using Unity.Entities;

namespace Spirit604.DotsCity.Core
{
    [UpdateInGroup(typeof(InitGroup), OrderFirst = true)]
    [BurstCompile]
    public partial struct CullEntitySystem : ISystem
    {
        private EntityQuery destroyQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            destroyQuery = SystemAPI.QueryBuilder()
                .WithNone<InViewOfCameraTag, InPermittedRangeTag>()
                .WithAll<WorldEntitySharedType, PoolableTag, CulledEventTag>()
                .Build();

            destroyQuery.SetSharedComponentFilter(new WorldEntitySharedType(EntityWorldType.PureEntity));

            state.RequireForUpdate(destroyQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var commandBuffer = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            var cullJob = new DestroyEntityJob()
            {
                commandBuffer = commandBuffer
            };

            cullJob.Schedule(destroyQuery);
        }
    }
}