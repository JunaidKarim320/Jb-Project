using Spirit604.DotsCity.Core;
using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [UpdateInGroup(typeof(LateInitGroup))]
    [BurstCompile]
    public partial struct UnloadOutOfCameraDummySkinSystem : ISystem
    {
        private EntityQuery unloadQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            unloadQuery = SystemAPI.QueryBuilder()
                .WithNone<InViewOfCameraTag>()
                .WithAllRW<DummySkinEnabledTag>()
                .WithAll<DummySkinData, LoadDummySkinInViewTag>()
                .Build();

            state.RequireForUpdate(unloadQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var unloadJob = new UnloadDummySkinJob()
            {
                MaterialMeshInfoLookup = SystemAPI.GetComponentLookup<MaterialMeshInfo>(false)
            };

            unloadJob.Schedule(unloadQuery);
        }
    }
}