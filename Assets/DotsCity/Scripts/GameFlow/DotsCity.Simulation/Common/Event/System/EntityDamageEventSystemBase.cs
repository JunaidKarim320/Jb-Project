using Unity.Entities;

namespace Spirit604.DotsCity.Events
{
    public abstract partial class EntityDamageEventSystemBase : EndSimulationSystemBase
    {
        protected EntityDamageEventConsumerSystem EntityDamageEventConsumerSystem { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            EntityDamageEventConsumerSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EntityDamageEventConsumerSystem>();
        }
    }
}