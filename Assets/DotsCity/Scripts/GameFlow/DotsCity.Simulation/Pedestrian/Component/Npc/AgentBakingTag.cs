using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
#if PROJECTDAWN_NAV
    public struct AgentBakingTag : IComponentData, IEnableableComponent { }
#endif
}