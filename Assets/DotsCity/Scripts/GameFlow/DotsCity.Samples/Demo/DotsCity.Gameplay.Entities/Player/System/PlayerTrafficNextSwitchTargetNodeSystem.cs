using Spirit604.DotsCity.Simulation.Traffic;
using Unity.Entities;

namespace Spirit604.DotsCity.Gameplay.Player
{
    [UpdateInGroup(typeof(InitGroup))]
    [RequireMatchingQueriesForUpdate]
    public partial class PlayerTrafficNextSwitchTargetNodeSystem : BeginInitSystemBase
    {
        private PlayerSpawnTrafficControlService playerTrafficControlService;

        protected override void OnUpdate()
        {
            var commandBuffer = GetCommandBuffer();

            Entities
            .WithoutBurst()
            .WithAll<TrafficNextTrafficNodeRequestTag, TrafficPlayerControlTag>()
            .ForEach((
                Entity entity) =>
            {
                commandBuffer.SetComponentEnabled<TrafficNextTrafficNodeRequestTag>(entity, false);
                playerTrafficControlService.UpdateNext();
            }).Run();

            AddCommandBufferForProducer();
        }

        public void Initialize(PlayerSpawnTrafficControlService playerTrafficControlService)
        {
            this.playerTrafficControlService = playerTrafficControlService;
        }
    }
}