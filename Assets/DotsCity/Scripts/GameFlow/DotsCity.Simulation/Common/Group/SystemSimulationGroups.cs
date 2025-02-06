using Unity.Entities;

namespace Spirit604.DotsCity.Simulation
{
    [UpdateInGroup(typeof(LateSimulationGroup))]
    public partial class PropSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(InitGroup))]
    public partial class PedestrianInitGroup : ComponentSystemGroup
    {
    }

    [UpdateAfter(typeof(CarSimulationGroup))]
    [UpdateInGroup(typeof(SimulationGroup))]
    public partial class PedestrianSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(FixedStepGroup))]
    public partial class PedestrianFixedSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateAfter(typeof(TrafficLateSimulationGroup))]
    [UpdateInGroup(typeof(LateSimulationGroup))]
    public partial class PedestrianLateSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(BeginPresentationGroup))]
    public partial class PedestrianPresentationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(PedestrianLateSimulationGroup))]
    public partial class PedestrianTriggerSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateAfter(typeof(HashMapGroup))]
    [UpdateInGroup(typeof(LateInitGroup))]
    public partial class NavSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(LateSimulationGroup))]
    public partial class TrafficInitGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(BeginSimulationGroup))]
    public partial class TrafficBeginSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(SimulationGroup))]
    public partial class TrafficSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(LateSimulationGroup))]
    public partial class TrafficLateSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(BeginPresentationGroup))]
    public partial class TrafficBeginPresentationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(LateSimulationGroup), OrderLast = true)]
    public partial class TrafficAreaSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(FixedStepGroup))]
    public partial class TrafficFixedUpdateGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(SimulationGroup))]
    public partial class CarSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(FixedStepGroup))]
    public partial class CarVisualDamageSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(SimulationGroup))]
    public partial class NpcSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(LateInitGroup))]
    public partial class HashMapGroup : ComponentSystemGroup
    {
    }
}