using Spirit604.DotsCity.Core;
using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine.Rendering;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [UpdateInGroup(typeof(LateInitGroup))]
    [BurstCompile]
    public partial struct LoadInCameraDummySkinSystem : ISystem, ISystemStartStop
    {
        private EntityQuery loadQuery;
        private BatchMaterialID materialBatchIdLocalRef;
        private BatchMeshID meshBatchIdLocalRef;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            loadQuery = SystemAPI.QueryBuilder()
                .WithNone<InPermittedRangeTag>()
                .WithDisabledRW<DummySkinEnabledTag>()
                .WithAll<DummySkinData, InViewOfCameraTag, LoadDummySkinInViewTag>()
                .Build();

            state.RequireForUpdate(loadQuery);
            state.RequireForUpdate<DummySkinProviderSystem.FactoryCreatedEventTag>();
        }

        void ISystem.OnDestroy(ref SystemState state)
        {
            materialBatchIdLocalRef = default;
            meshBatchIdLocalRef = default;
        }

        public void OnStartRunning(ref SystemState state)
        {
            materialBatchIdLocalRef = DummySkinProviderSystem.MaterialBatchIdStaticRef;
            meshBatchIdLocalRef = DummySkinProviderSystem.MeshBatchIdStaticRef;
        }

        public void OnStopRunning(ref SystemState state) { }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var loadJob = new LoadDummySkinJob()
            {
                MaterialMeshInfoLookup = SystemAPI.GetComponentLookup<MaterialMeshInfo>(false),
                MaterialBatchId = materialBatchIdLocalRef,
                MeshBatchId = meshBatchIdLocalRef
            };

            loadJob.Schedule(loadQuery);
        }
    }
}