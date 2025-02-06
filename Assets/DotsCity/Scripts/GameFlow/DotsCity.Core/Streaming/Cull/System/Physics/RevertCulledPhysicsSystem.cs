using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.GraphicsIntegration;
using Unity.Transforms;

namespace Spirit604.DotsCity.Core
{
    [UpdateInGroup(typeof(LateInitGroup))]
    [BurstCompile]
    public partial struct RevertCulledPhysicsSystem : ISystem
    {
        private SystemHandle calcCullingSystem;
        private EntityQuery revertQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            calcCullingSystem = state.WorldUnmanaged.GetExistingUnmanagedSystem<CalcCullingSystem>();

            revertQuery = SystemAPI.QueryBuilder()
                .WithNone<InPermittedRangeTag, CulledEventTag, CustomCullPhysicsTag>()
                .WithAny<InViewOfCameraTag, PreInitInCameraTag>()
                .WithAll<CullPhysicsTag, PhysicsWorldIndex>()
                .Build();

            revertQuery.SetSharedComponentFilter(new PhysicsWorldIndex() { Value = ProjectConstants.NoPhysicsWorldIndex });

            state.RequireForUpdate(revertQuery);
            state.Enabled = false;
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            ref var calcCullingSystemRef = ref state.WorldUnmanaged.ResolveSystemStateRef(calcCullingSystem);

            var revertPhysicsJob = new RevertPhysicsJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
                VelocityLookup = SystemAPI.GetComponentLookup<VelocityComponent>(true),
                PhysicsVelocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>(true),
                StaticLookup = SystemAPI.GetComponentLookup<Static>(true),
                PhysicsGraphicalInterpolationLookup = SystemAPI.GetComponentLookup<PhysicsGraphicalInterpolationBuffer>(true)
            };

            state.Dependency = revertPhysicsJob.Schedule(revertQuery, calcCullingSystemRef.Dependency);
        }
    }
}