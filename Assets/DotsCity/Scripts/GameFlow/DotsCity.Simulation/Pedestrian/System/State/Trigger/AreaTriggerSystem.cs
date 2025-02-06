using Spirit604.DotsCity.Core;
using Spirit604.Extensions;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [UpdateInGroup(typeof(PedestrianTriggerSimulationGroup))]
    [BurstCompile]
    public partial struct AreaTriggerSystem : ISystem, ISystemStartStop
    {
        #region Helper types

        public struct AreaTriggerInfo
        {
            public readonly Entity NpcEntity;
            public readonly float3 TriggerPosition;
            public readonly TriggerAreaType TriggerAreaType;

            public AreaTriggerInfo(Entity npcEntity, float3 triggerPosition, TriggerAreaType triggerAreaType)
            {
                NpcEntity = npcEntity;
                TriggerPosition = triggerPosition;
                TriggerAreaType = triggerAreaType;
            }
        }

        #endregion

        #region Variables

        private EntityQuery triggerQuery;
        private EntityQuery pedestrianQuery;
        private NativeParallelMultiHashMap<int, TriggerComponent> triggerHashMap;

        #endregion

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            triggerQuery = SystemAPI.QueryBuilder()
                .WithAll<TriggerComponent>()
                .Build();

            pedestrianQuery = SystemAPI.QueryBuilder()
                .WithAll<DestinationComponent>()
                .Build();

            var configQuery = SystemAPI.QueryBuilder()
                .WithAll<TriggerConfigReference>()
                .Build();

            state.RequireForUpdate(triggerQuery);
            state.RequireForUpdate(configQuery);

            state.Enabled = false;
        }

        void ISystem.OnDestroy(ref SystemState state)
        {
            Dispose();
        }

        void ISystemStartStop.OnStartRunning(ref SystemState state)
        {
            var triggerConfigReference = SystemAPI.GetSingleton<TriggerConfigReference>().Config;

            if (!triggerHashMap.IsCreated)
            {
                triggerHashMap = new NativeParallelMultiHashMap<int, TriggerComponent>(triggerConfigReference.Value.TriggerHashMapCapacity, Allocator.Persistent);
            }
        }

        void ISystemStartStop.OnStopRunning(ref SystemState state) { }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            if (!triggerHashMap.IsCreated)
            {
                return;
            }

            triggerHashMap.Clear();

            var triggerCount = triggerQuery.CalculateEntityCount();

            if (triggerHashMap.Capacity < triggerCount)
            {
                triggerHashMap.Capacity = triggerCount;
            }

            if (triggerCount == 0)
            {
                return;
            }

            var config = SystemAPI.GetSingleton<TriggerConfigReference>();

            var fillHashMapJob = new FillHashMapJob()
            {
                hashMapParallel = triggerHashMap.AsParallelWriter(),
                TriggerConfigReference = config,
            };

            JobHandle fillHashMapHandle = fillHashMapJob.ScheduleParallel(state.Dependency);

            var calcTriggerDistanceJob = new CalcTriggerDistanceJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                TriggerHashMap = triggerHashMap,
                PooledEventLookup = SystemAPI.GetComponentLookup<PooledEventTag>(true),
                TriggerConfigReference = config,
            };

            state.Dependency = calcTriggerDistanceJob.ScheduleParallel(fillHashMapHandle);
        }

        [BurstCompile]
        public partial struct FillHashMapJob : IJobEntity
        {
            [NativeDisableContainerSafetyRestriction]
            public NativeParallelMultiHashMap<int, TriggerComponent>.ParallelWriter hashMapParallel;

            [ReadOnly]
            public TriggerConfigReference TriggerConfigReference;

            void Execute(
                in TriggerComponent pedestrianTriggerComponent)
            {
                var triggerPosition = pedestrianTriggerComponent.Position.Flat();

                var key = HashMapHelper.GetHashMapPosition(triggerPosition, TriggerConfigReference.Config.Value.TriggerHashMapCellSize);

                hashMapParallel.Add(key, pedestrianTriggerComponent);
            }
        }

        [WithNone(typeof(HasImpactTriggerTag))]
        [BurstCompile]
        public partial struct CalcTriggerDistanceJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter CommandBuffer;

            [ReadOnly]
            public NativeParallelMultiHashMap<int, TriggerComponent> TriggerHashMap;

            [ReadOnly]
            public ComponentLookup<PooledEventTag> PooledEventLookup;

            [ReadOnly]
            public TriggerConfigReference TriggerConfigReference;

            void Execute(
                Entity entity,
                [ChunkIndexInQuery] int entityInQueryIndex,
                in LocalToWorld worldTransform)
            {
                float3 pedestrianPosition = worldTransform.Position.Flat();

                var keys = HashMapHelper.GetHashMapPosition9Cells(pedestrianPosition, TriggerConfigReference.Config.Value.TriggerHashMapCellSize, TriggerConfigReference.Config.Value.TriggerHashMapCellSize);

                bool found = false;

                for (int i = 0; i < keys.Length; i++)
                {
                    if (TriggerHashMap.TryGetFirstValue(keys[i], out var hashEntity, out var nativeMultiHashMapIterator))
                    {
                        do
                        {
                            float distance = math.distancesq(worldTransform.Position, hashEntity.Position);

                            if (distance < hashEntity.TriggerDistanceSQ)
                            {
                                var trigger = new AreaTriggerInfo(entity, hashEntity.Position, hashEntity.TriggerAreaType);

                                AreaTriggerUtils.AddTrigger(
                                    ref CommandBuffer,
                                    entityInQueryIndex,
                                    in PooledEventLookup,
                                    in TriggerConfigReference,
                                    entity,
                                    in trigger);

                                found = true;
                                break;
                            }
                        } while (TriggerHashMap.TryGetNextValue(out hashEntity, ref nativeMultiHashMapIterator));
                    }

                    if (found)
                    {
                        break;
                    }
                }

                keys.Dispose();
            }
        }

        private void Dispose()
        {
            if (triggerHashMap.IsCreated)
            {
                triggerHashMap.Dispose();
            }
        }
    }
}