﻿using Spirit604.DotsCity.Core;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Hybrid.Core
{
    [UpdateInGroup(typeof(InitGroup), OrderFirst = true)]
    public partial struct PoolCustomHybridEntitySystem : ISystem
    {
        private EntityQuery updateQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            updateQuery = SystemAPI.QueryBuilder()
                .WithAll<WorldEntitySharedType, PooledEventTag, PoolableTag>()
                .Build();

            updateQuery.SetSharedComponentFilter(new WorldEntitySharedType(EntityWorldType.HybridRuntimeEntity));

            state.RequireForUpdate(updateQuery);
        }

        void ISystem.OnUpdate(ref SystemState state)
        {
            var commandBuffer = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (transform, entity) in SystemAPI.Query<SystemAPI.ManagedAPI.UnityEngineComponent<Transform>>()
                .WithAll<WorldEntitySharedType, PooledEventTag, PoolableTag>()
                .WithSharedComponentFilter(new WorldEntitySharedType { EntityWorldType = EntityWorldType.HybridRuntimeEntity })
                .WithEntityAccess())
            {
                var hybridEntity = transform.Value.GetComponent<HybridEntityRuntimeAuthoring>();

                if (hybridEntity != null)
                {
                    hybridEntity.Destroyed = true;
                    hybridEntity.ResetEntity();
                    RuntimeHybridEntityService.Instance.DestroyEntity(hybridEntity);
                }

                commandBuffer.DestroyEntity(entity);
            }
        }
    }
}