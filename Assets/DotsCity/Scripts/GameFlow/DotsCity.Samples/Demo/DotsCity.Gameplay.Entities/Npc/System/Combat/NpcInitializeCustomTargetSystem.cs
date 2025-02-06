using Spirit604.DotsCity.Simulation;
using Unity.Entities;

#if REESE_PATH
using Reese.Path;
using Spirit604.DotsCity.Simulation.Npc.Navigation;
using Spirit604.Extensions;
#endif

namespace Spirit604.DotsCity.Gameplay.Npc
{
    [UpdateInGroup(typeof(NpcSimulationGroup))]
    [RequireMatchingQueriesForUpdate]
    public partial class NpcInitializeCustomTargetSystem : EndSimulationSystemBase
    {
        protected override void OnUpdate()
        {
#if REESE_PATH
            var commandBuffer = GetCommandBuffer();

            Entities
            .WithoutBurst()
            .WithNone<PathPlanning, UpdateNavTargetTag>()
            .WithAll<NpcInitializeCustomDestinationTag>()
            .ForEach((
                Entity entity,
                ref NavAgentComponent navMeshAgent,
                ref NpcCustomDestinationComponent npcCustomTargetComponent) =>
            {
                commandBuffer.RemoveComponent<NpcInitializeCustomDestinationTag>(entity);

                if (!navMeshAgent.PathEndPosition.IsEqual(npcCustomTargetComponent.Destination, 0.1f))
                {
                    navMeshAgent.PathEndPosition = npcCustomTargetComponent.Destination;
                    commandBuffer.SetComponentEnabled<UpdateNavTargetTag>(entity, true);
                }
            }).Schedule();

            AddCommandBufferForProducer();
#endif
        }
    }
}