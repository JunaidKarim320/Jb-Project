using Spirit604.AnimationBaker.Entities;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [UpdateInGroup(typeof(PedestrianInitGroup))]
    public partial class InitGPUSkinSystem : SystemBase
    {
        private CrowdSkinProviderSystem crowdSkinProviderSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            crowdSkinProviderSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CrowdSkinProviderSystem>();
            crowdSkinProviderSystem.OnInitialized += CrowdSkinProviderSystem_OnInitialized;
            Enabled = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            crowdSkinProviderSystem.OnInitialized -= CrowdSkinProviderSystem_OnInitialized;
        }

        protected override void OnUpdate() { }

        private void CrowdSkinProviderSystem_OnInitialized()
        {
            var prefabQuery = EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<GPUSkinTag>(),
                ComponentType.ReadOnly<Prefab>(),
                ComponentType.ReadOnly<SkinAnimatorData>(),
                ComponentType.Exclude<RenderMeshArray>());

            var entities = prefabQuery.ToEntityArray(Allocator.TempJob);
            var renderMeshArray = crowdSkinProviderSystem.TotalRenderMeshData;

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var skinAnimatorData = EntityManager.GetComponentData<SkinAnimatorData>(entity);

                EntityManager.AddSharedComponentManaged(entity, renderMeshArray);

                var materialMeshInfo = MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0);

                materialMeshInfo.MeshID = crowdSkinProviderSystem.GetDefaultMeshBatchId(skinAnimatorData.SkinIndex);
                materialMeshInfo.MaterialID = crowdSkinProviderSystem.GetDefaultMaterialBatchId(skinAnimatorData.SkinIndex);

                EntityManager.SetComponentData(entity, materialMeshInfo);
            }

            entities.Dispose();
        }
    }
}