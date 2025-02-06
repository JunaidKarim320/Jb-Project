using Unity.Entities;

namespace Spirit604.DotsCity.Gameplay
{
    [UpdateInGroup(typeof(InitGroup))]
    public partial class PlayerInitGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(SimulationGroup))]
    public partial class PlayerSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(LateSimulationGroup))]
    public partial class PlayerLateSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class PlayerPresentationGroup : ComponentSystemGroup
    {
    }
}