using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Binding
{
    [UpdateInGroup(typeof(InitGroup))]
    [RequireMatchingQueriesForUpdate]
    public partial class EntityBindingSystem : BeginInitSystemBase
    {
        private EntityBindingService entityBindingService;

        protected override void OnCreate()
        {
            base.OnCreate();
            Enabled = false;
        }

        protected override void OnUpdate()
        {
            var commandBuffer = GetCommandBuffer();

            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .WithAll<EntityIDInitTag>()
            .ForEach((
                Entity entity,
                in EntityID entityID) =>
            {
                entityBindingService.RegisterEntity(entity, entityID.Value);
                commandBuffer.SetComponentEnabled<EntityIDInitTag>(entity, false);

            }).Run();

            AddCommandBufferForProducer();
        }

        public void Initialize(EntityBindingService entityBindingService)
        {
            this.entityBindingService = entityBindingService;
            Enabled = true;
        }
    }
}