using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Traffic
{
    public struct TrafficParkingConfig
    {
        public bool AligmentAtNode;
        public float RotationSpeed;
        public float CompleteAngle;
        public bool PrecisePosition;
        public float MovementSpeed;
        public float AchieveDistanceSQ;
    }

    public struct TrafficParkingConfigReference : IComponentData
    {
        public BlobAssetReference<TrafficParkingConfig> Config;
    }
}