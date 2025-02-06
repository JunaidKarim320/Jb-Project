using Spirit604.Attributes;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Sound
{
    public class SoundLevelConfigHolder : MonoBehaviour
    {
        [DocLinker("https://dotstrafficcity.readthedocs.io/en/latest/commonConfigs.html#common-sound-config")]
        [SerializeField] private string link;

        [Expandable]
        [SerializeField] private SoundLevelConfig soundLevelConfig;

        public SoundLevelConfig SoundLevelConfig => soundLevelConfig;

        public class SoundLevelConfigBaker : Baker<SoundLevelConfigHolder>
        {
            public override void Bake(SoundLevelConfigHolder authoring)
            {
                var entity = CreateAdditionalEntity(TransformUsageFlags.None);

                var hasSounds = authoring.soundLevelConfig?.HasSounds ?? false;

                using (var builder = new BlobBuilder(Unity.Collections.Allocator.Temp))
                {
                    ref var root = ref builder.ConstructRoot<SoundLevelConfigData>();

                    root.HasSounds = hasSounds;

                    var blobRef = builder.CreateBlobAssetReference<SoundLevelConfigData>(Unity.Collections.Allocator.Persistent);

                    AddBlobAsset(ref blobRef, out var hash);

                    AddComponent(entity, new SoundLevelConfigReference() { Config = blobRef });
                }
            }
        }
    }
}