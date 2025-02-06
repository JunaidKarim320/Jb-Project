using Unity.Entities;
using Unity.Mathematics;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    public struct TriggerComponent : IComponentData
    {
        public float3 Position;
        public float TriggerDistanceSQ;
        public TriggerAreaType TriggerAreaType;
    }

    public struct ImpactTriggerData : IComponentData
    {
        public float Duration;
        public float EndTime;
    }

    public struct HasImpactTriggerTag : IComponentData { }

    public enum TriggerAreaType
    {
        Default = 0,
        FearPointTrigger = 1
    }
}
