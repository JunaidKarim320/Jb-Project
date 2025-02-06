using Spirit604.DotsCity.Simulation.Common;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine.Rendering;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [UpdateInGroup(typeof(InitGroup))]
    public partial class DummySkinProviderSystem : SystemBase
    {
        public struct FactoryCreatedEventTag : IComponentData { };

        private EntitiesGraphicsSystem entitiesGraphicsSystem;
        private BatchMaterialID materialBatchId;
        private BatchMeshID meshBatchId;

        public BatchMaterialID MaterialBatchId { get => materialBatchId; set => materialBatchId = value; }
        public BatchMeshID MeshBatchId { get => meshBatchId; set => meshBatchId = value; }

        public static BatchMaterialID MaterialBatchIdStaticRef { get; private set; }
        public static BatchMeshID MeshBatchIdStaticRef { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            entitiesGraphicsSystem = World.GetOrCreateSystemManaged<EntitiesGraphicsSystem>();
            Enabled = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            MaterialBatchIdStaticRef = default;
            MeshBatchIdStaticRef = default;
        }

        protected override void OnUpdate() { }

        public void Initialize(MyRenderMesh renderMeshPrefabComponent)
        {
            //MaterialBatchId = entitiesGraphicsSystem.RegisterMaterial(renderMeshPrefabComponent.material);
            //MeshBatchId = entitiesGraphicsSystem.RegisterMesh(renderMeshPrefabComponent.mesh);
            //MaterialBatchIdStaticRef = MaterialBatchId;
            //MeshBatchIdStaticRef = MeshBatchId;

            EntityManager.CreateEntity(typeof(FactoryCreatedEventTag));
        }
    }
}