using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.GraphicsIntegration;

namespace Spirit604.DotsCity.Core
{
    [UpdateInGroup(typeof(LateInitGroup))]
    [BurstCompile]
    public partial struct CullPhysicsSystem : ISystem
    {
        private SystemHandle calcCullingSystem;
        private EntityQuery cullQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            calcCullingSystem = state.WorldUnmanaged.GetExistingUnmanagedSystem<CalcCullingSystem>();

            cullQuery = SystemAPI.QueryBuilder()
                .WithNone<InViewOfCameraTag>()
                .WithAny<InPermittedRangeTag, CulledEventTag>()
                .WithAll<PhysicsVelocity, PhysicsWorldIndex, CullPhysicsTag>()
                .Build();

            cullQuery.SetSharedComponentFilter(new PhysicsWorldIndex() { Value = 0 });

            state.RequireForUpdate(cullQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            ref var calcCullingSystemRef = ref state.WorldUnmanaged.ResolveSystemStateRef(calcCullingSystem);

            var depJob = calcCullingSystemRef.Dependency;

            var cullPhysicsJob = new CullPhysicsJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                PhysicsGraphicalSmoothingLookup = SystemAPI.GetComponentLookup<PhysicsGraphicalSmoothing>(true)
            };

            state.Dependency = cullPhysicsJob.ScheduleParallel(cullQuery, depJob);
        }
    }

    [WithNone(typeof(InViewOfCameraTag))]
    [WithAny(typeof(InPermittedRangeTag), typeof(CulledEventTag))]
    [WithAll(typeof(PhysicsVelocity), typeof(PhysicsWorldIndex), typeof(CullPhysicsTag))]
    [BurstCompile]
    public partial struct CullPhysicsJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter CommandBuffer;

        [ReadOnly]
        public ComponentLookup<PhysicsGraphicalSmoothing> PhysicsGraphicalSmoothingLookup;

        void Execute(
            [ChunkIndexInQuery] int entityInQueryIndex,
            Entity entity)
        {
            CommandBuffer.SetSharedComponent(entityInQueryIndex, entity, new PhysicsWorldIndex()
            {
                Value = ProjectConstants.NoPhysicsWorldIndex
            });

            if (PhysicsGraphicalSmoothingLookup.HasComponent(entity))
            {
                CommandBuffer.RemoveComponent<PhysicsGraphicalSmoothing>(entityInQueryIndex, entity);
            }
        }
    }
}