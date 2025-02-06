using Spirit604.DotsCity.Simulation.Common;
using Spirit604.DotsCity.Simulation.Npc;
using Spirit604.DotsCity.Simulation.Npc.Navigation;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Pedestrian.Authoring
{
    public class PedestrianSettingsBaker : Baker<PedestrianSpawnerConfigHolder>
    {
        public override void Bake(PedestrianSpawnerConfigHolder authoring)
        {
            MyRenderMesh dummyRenderMesh = default;

            var dummySkinPrefab = authoring.DummySkinPrefab;
            float scale = 1f;

            if (dummySkinPrefab != null)
            {
                var meshFilter = dummySkinPrefab.GetComponentInChildren<MeshFilter>();
                var meshRenderer = dummySkinPrefab.GetComponentInChildren<MeshRenderer>();

                if (meshFilter && meshRenderer)
                {
                    dummyRenderMesh = new MyRenderMesh() { material = meshRenderer.sharedMaterial, mesh = meshFilter.sharedMesh };
                }

                scale = dummySkinPrefab.transform.localScale.x;
            }

            var entity = CreateAdditionalEntity(TransformUsageFlags.None);

            AddComponent(entity, PedestrianMiscConversionSettingsAuthoring.CreateConfigStatic(this, authoring));

            var dummyPrefabDataEntity = CreateAdditionalEntity(TransformUsageFlags.None);

            AddComponent(dummyPrefabDataEntity, new DummyPrefabData()
            {
                Scale = scale
            });

            AddSharedComponentManaged(dummyPrefabDataEntity, dummyRenderMesh);
        }
    }

    public struct MiscConversionSettings
    {
        public NpcSkinType PedestrianSkinType;
        public float PedestrianColliderRadius;
        public bool HasRig;
        public NpcRigType PedestrianRigType;
        public EntityType EntityType;
        public NpcNavigationType PedestrianNavigationType;
        public ObstacleAvoidanceType ObstacleAvoidanceType;
        public CollisionType CollisionType;
        public bool HasRagdoll;
        public RagdollType RagdollType;
        public bool AutoAddAgentComponents;

        public bool DefaultRagdollSystem => RagdollType == RagdollType.Default;
    }

    public struct MiscConversionSettingsReference : IComponentData
    {
        public BlobAssetReference<MiscConversionSettings> Config;
    }

    public struct DummyPrefabData : IComponentData
    {
        public float Scale;
    }
}
