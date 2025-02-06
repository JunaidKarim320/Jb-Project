using Spirit604.Attributes;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Npc.Authoring
{
    public class NpcCommonConfigAuthoring : MonoBehaviour
    {
        [DocLinker("https://dotstrafficcity.readthedocs.io/en/latest/npc.html#npc-common-config")]
        [SerializeField] private string link;

        [Expandable]
        [SerializeField] private NpcCommonSettingsConfig npcCommonSettingsConfig;

        class NpcCommonConfigAuthoringBaker : Baker<NpcCommonConfigAuthoring>
        {
            public override void Bake(NpcCommonConfigAuthoring authoring)
            {
                var entity = CreateAdditionalEntity(TransformUsageFlags.None);

                using (var builder = new BlobBuilder(Unity.Collections.Allocator.Temp))
                {
                    ref var root = ref builder.ConstructRoot<NpcCommonConfig>();

                    root.NpcHashMapCapacity = authoring.npcCommonSettingsConfig.NpcHashMapCapacity;

                    var blobRef = builder.CreateBlobAssetReference<NpcCommonConfig>(Unity.Collections.Allocator.Persistent);

                    AddBlobAsset(ref blobRef, out var hash);

                    AddComponent(entity, new NpcCommonConfigReference() { Config = blobRef });
                }
            }
        }
    }
}
