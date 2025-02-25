using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    public struct PedestrianGeneralSettingsData
    {
        public EntityBakingType EntityBakingType;
        public bool HasPedestrian;
        public bool NavigationSupport;
        public bool TriggerSupport;
    }

    public struct PedestrianGeneralSettingsReference : IComponentData
    {
        public BlobAssetReference<PedestrianGeneralSettingsData> Config;
    }
}
