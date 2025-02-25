using Spirit604.Attributes;
using Spirit604.DotsCity.Core;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Traffic.Authoring
{
    public class TrafficSpawnerSettingsAuthoring : RuntimeConfigUpdater<TrafficSpawnerConfigBlobReference, TrafficSpawnerConfigBlob>
    {
        [ShowIfNull]
        [SerializeField] private TrafficSettings trafficSettings;

        protected override bool UpdateAvailableByDefault => false;
        protected override bool AutoSync => false;

        public override void Initialize(bool recreateOnStart)
        {
            base.Initialize(recreateOnStart);

#if UNITY_EDITOR
            trafficSettings.TrafficSpawnerConfig.OnSettingsChanged += TrafficSpawnerConfig_OnSettingsChanged;
#endif
        }

        public override TrafficSpawnerConfigBlobReference CreateConfig(BlobAssetReference<TrafficSpawnerConfigBlob> blobRef)
        {
            return new TrafficSpawnerConfigBlobReference()
            {
                Reference = CreateBlobStatic(trafficSettings)
            };
        }

        protected override BlobAssetReference<TrafficSpawnerConfigBlob> CreateConfigBlob()
        {
            return CreateBlobStatic(trafficSettings);
        }

        public static TrafficSpawnerConfigBlobReference CreateConfigStatic(IBaker baker, TrafficSettings trafficSettings)
        {
            var blobRef = CreateBlobStatic(trafficSettings);

            baker.AddBlobAsset(ref blobRef, out var hash);

            return new TrafficSpawnerConfigBlobReference()
            {
                Reference = blobRef
            };
        }

        private static BlobAssetReference<TrafficSpawnerConfigBlob> CreateBlobStatic(TrafficSettings trafficSettings)
        {
            var spawnerConfig = trafficSettings.TrafficSpawnerConfig;

            using (var builder = new BlobBuilder(Unity.Collections.Allocator.Temp))
            {
                ref var root = ref builder.ConstructRoot<TrafficSpawnerConfigBlob>();

                root.PreferableCount = spawnerConfig.PreferableCount;
                root.MaxCarsPerNode = spawnerConfig.MaxCarsPerNode;

                var hashMapCapacity = spawnerConfig.HashMapCapacity;

                if (hashMapCapacity < root.PreferableCount)
                {
                    hashMapCapacity = Mathf.FloorToInt((float)root.PreferableCount * 1.2f);
                }

                root.HashMapCapacity = hashMapCapacity;
                root.MaxSpawnCountByIteration = spawnerConfig.MaxSpawnCountByIteration;
                root.MaxParkingCarsCount = spawnerConfig.MaxParkingCarsCount;
                root.MinSpawnDelay = spawnerConfig.MinSpawnDelay;
                root.MaxSpawnDelay = spawnerConfig.MaxSpawnDelay;
                root.MinSpawnCarDistance = spawnerConfig.MinSpawnCarDistance;
                root.MinSpawnCarDistanceSQ = spawnerConfig.MinSpawnCarDistance * spawnerConfig.MinSpawnCarDistance;

                return builder.CreateBlobAssetReference<TrafficSpawnerConfigBlob>(Unity.Collections.Allocator.Persistent);
            }
        }

        protected override void OnConfigUpdatedInternal()
        {
            base.OnConfigUpdatedInternal();
            World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<TrafficSpawnerSystem>().UpdateSpawnConfig();
        }

#if UNITY_EDITOR
        private void TrafficSpawnerConfig_OnSettingsChanged()
        {
            ConfigUpdated = true;
        }
#endif
    }
}
