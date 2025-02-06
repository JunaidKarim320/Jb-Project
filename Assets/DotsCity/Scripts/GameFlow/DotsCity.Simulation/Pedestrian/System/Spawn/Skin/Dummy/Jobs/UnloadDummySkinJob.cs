using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [BurstCompile]
    public partial struct UnloadDummySkinJob : IJobEntity
    {
        public ComponentLookup<MaterialMeshInfo> MaterialMeshInfoLookup;

        void Execute(
            EnabledRefRW<DummySkinEnabledTag> dummySkinEnabledTagRW,
            in DummySkinData dummySkinData)
        {
            if (dummySkinData.UnloadRenderBounds)
            {
                MaterialMeshInfoLookup.SetComponentEnabled(dummySkinData.DummyEntity, false);
            }

            dummySkinEnabledTagRW.ValueRW = false;
        }
    }
}