using Spirit604.DotsCity.Simulation.Pedestrian.State;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [RequireMatchingQueriesForUpdate]
    [UpdateAfter(typeof(StopTalkStateSystem))]
    [UpdateInGroup(typeof(PedestrianSimulationGroup))]
    public partial class DisableTalkStateSystem : EndSimulationSystemBase
    {
        protected override void OnUpdate()
        {
            var commandBuffer = GetCommandBuffer();

            Entities
            .WithoutBurst()
            .WithNone<TalkComponent>()
            .WithAll<TalkAreaComponent>()
            .ForEach((
                Entity entity) =>
            {
                commandBuffer.RemoveComponent<TalkAreaComponent>(entity);
            }).Schedule();

            AddCommandBufferForProducer();
        }
    }
}