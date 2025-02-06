using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine.Rendering;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [BurstCompile]
    public partial struct LoadDummySkinJob : IJobEntity
    {
        public ComponentLookup<MaterialMeshInfo> MaterialMeshInfoLookup;

        [ReadOnly]
        public BatchMaterialID MaterialBatchId;

        [ReadOnly]
        public BatchMeshID MeshBatchId;

        void Execute(
            EnabledRefRW<DummySkinEnabledTag> dummySkinEnabledTagRW,
            in DummySkinData dummySkinData)
        {
            var materialMeshInfo = MaterialMeshInfoLookup[dummySkinData.DummyEntity];

            materialMeshInfo.MaterialID = MaterialBatchId;
            materialMeshInfo.MeshID = MeshBatchId;

            MaterialMeshInfoLookup[dummySkinData.DummyEntity] = materialMeshInfo;
            MaterialMeshInfoLookup.SetComponentEnabled(dummySkinData.DummyEntity, true);

            dummySkinEnabledTagRW.ValueRW = true;
        }
    }
}