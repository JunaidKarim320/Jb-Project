using Spirit604.Extensions;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Core
{
    [UpdateInGroup(typeof(InitGroup), OrderFirst = true)]
    public partial struct CullLinkedHybridEntitySystem : ISystem
    {
        private EntityQuery updateQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            updateQuery = SystemAPI.QueryBuilder()
                .WithNone<InPermittedRangeTag>()
                .WithAll<WorldEntitySharedType, CulledEventTag, PoolableTag>()
                .Build();

            updateQuery.SetSharedComponentFilter(new WorldEntitySharedType(EntityWorldType.LinkedHybridEntity));

            state.RequireForUpdate(updateQuery);
        }

        void ISystem.OnUpdate(ref SystemState state)
        {
            var commandBuffer = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (transform, entity) in SystemAPI.Query<SystemAPI.ManagedAPI.UnityEngineComponent<Transform>>()
                .WithNone<InPermittedRangeTag>()
                .WithAll<WorldEntitySharedType, CulledEventTag, PoolableTag>()
                .WithSharedComponentFilter(new WorldEntitySharedType { EntityWorldType = EntityWorldType.LinkedHybridEntity })
                .WithEntityAccess())
            {
                var hybridLink = transform.Value.GetComponent<IHybridLinkEntity>();

                if (hybridLink != null)
                {
                    hybridLink.Destroy();
                }
#if UNITY_EDITOR
                else
                {
                    Debug.Log($"CullLinkedHybridEntitySystem. Vehicle '{transform.Value.gameObject.name}' doesn't have IHybridLinkEntity interface.");
                }
#endif

                transform.Value.gameObject.ReturnToPool();
                commandBuffer.DestroyEntity(entity);
            }
        }
    }
}