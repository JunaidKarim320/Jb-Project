using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Gameplay.Weapon
{
    [UpdateInGroup(typeof(PlayerSimulationGroup))]
    [RequireMatchingQueriesForUpdate]
    public partial class CrossHairScaleSystem : EndSimulationSystemBase
    {
        protected override void OnUpdate()
        {
            var commandBuffer = GetCommandBuffer();

            Entities
                .WithoutBurst()
                .WithAll<CrossHairUpdateScaleTag>()
                .ForEach((
            Entity entity,
            ref CrossHairComponent crossHairComponent) =>
            {
                if (crossHairComponent.CurrentScale != crossHairComponent.TargetScale)
                {
                    crossHairComponent.CurrentScale = crossHairComponent.TargetScale;

                    var transform = EntityManager.GetComponentObject<Transform>(entity);

                    transform.localScale = new Vector3(crossHairComponent.CurrentScale, crossHairComponent.CurrentScale, crossHairComponent.CurrentScale);
                }

                commandBuffer.RemoveComponent(entity, typeof(CrossHairUpdateScaleTag));
            }).Run();

            AddCommandBufferForProducer();
        }
    }
}
