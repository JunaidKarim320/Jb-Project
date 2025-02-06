using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Spirit604.DotsCity
{
    [UpdateBefore(typeof(BeginInitializationEntityCommandBufferSystem))]
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public partial class InitGroup : ComponentSystemGroup
    {
    }

    [UpdateBefore(typeof(EndInitializationEntityCommandBufferSystem))]
    [UpdateAfter(typeof(BeginInitializationEntityCommandBufferSystem))]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class LateInitGroup : ComponentSystemGroup
    {
    }

    [UpdateBefore(typeof(BeginSimulationEntityCommandBufferSystem))]
    [UpdateInGroup(typeof(SimulationGroup), OrderFirst = true)]
    public partial class BeginSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class SimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class BeforeTransformGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class FixedStepGroup : ComponentSystemGroup
    {
    }

    [UpdateAfter(typeof(PhysicsInitializeGroup)), UpdateBefore(typeof(PhysicsSimulationGroup))]
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    public partial class RaycastGroup : ComponentSystemGroup
    {
    }

    [UpdateBefore(typeof(PhysicsSystemGroup))]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class PhysicsSimGroup : ComponentSystemGroup
    {
    }

    [UpdateAfter(typeof(PhysicsSystemGroup))]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class PhysicsTriggerGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class LateSimulationGroup : ComponentSystemGroup
    {
    }

    [UpdateBefore(typeof(BeginPresentationEntityCommandBufferSystem))]
    [UpdateInGroup(typeof(PresentationSystemGroup), OrderFirst = true)]
    public partial class BeginPresentationGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(InitGroup))]
    public partial class CullSimulationGroup : ComponentSystemGroup
    {
    }
}