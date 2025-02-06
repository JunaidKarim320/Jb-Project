using Spirit604.DotsCity.Simulation;
using Unity.Entities;

namespace Spirit604.DotsCity.Gameplay.Npc
{
    [UpdateInGroup(typeof(NpcSimulationGroup))]
    [RequireMatchingQueriesForUpdate]
    public partial class NpcResetCustomTargetSystem : EndSimulationSystemBase
    {
        protected override void OnUpdate()
        {
            var commandBuffer1 = GetCommandBuffer();
            var commandBuffer2 = GetCommandBuffer();

            Entities
            .WithoutBurst()
            .WithAll<NpcCustomDestinationComponent, ResetNpcCustomDestinationTag>()
            .ForEach((
                Entity entity) =>
            {
                commandBuffer1.RemoveComponent<NpcCustomDestinationComponent>(entity);
                commandBuffer1.RemoveComponent<ResetNpcCustomDestinationTag>(entity);
            }).Schedule();

            Entities
            .WithoutBurst()
            .WithAll<NpcCustomReachComponent, ResetNpcCustomDestinationTag>()
            .ForEach((
            Entity entity) =>
            {
                commandBuffer2.RemoveComponent<NpcCustomReachComponent>(entity);
                commandBuffer2.RemoveComponent<ResetNpcCustomDestinationTag>(entity);
            }).Schedule();

            AddCommandBufferForProducer();
        }
    }
}