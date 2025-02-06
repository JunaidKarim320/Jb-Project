using Unity.Entities;

namespace Spirit604.DotsCity.Debug
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(LateInitGroup))]
    public partial class TrafficRoadDebuggerCleanupSystem : EndInitSystemBase
    {
        private TrafficRoadSpawnDebuggerSystem trafficRoadDebuggerSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            trafficRoadDebuggerSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<TrafficRoadSpawnDebuggerSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = GetCommandBuffer();

            Entities
            .WithoutBurst()
            .WithNone<TrafficRoadDebuggerInfo>()
            .WithAll<TrafficRoadDebuggerCleanup>()
            .ForEach((
                Entity entity) =>
            {
                trafficRoadDebuggerSystem.RemoveDebugger(entity);
                commandBuffer.RemoveComponent<TrafficRoadDebuggerCleanup>(entity);
            }).Run();

            AddCommandBufferForProducer();
        }
    }
}
