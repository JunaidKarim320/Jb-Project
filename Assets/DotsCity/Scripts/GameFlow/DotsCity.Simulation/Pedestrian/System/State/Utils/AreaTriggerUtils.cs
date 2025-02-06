using Spirit604.DotsCity.Core;
using System.Runtime.CompilerServices;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    public static class AreaTriggerUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddTrigger(
            ref EntityCommandBuffer.ParallelWriter commandBuffer,
            int entityInQueryIndex,
            in ComponentLookup<PooledEventTag> pooledEventLookup,
            in TriggerConfigReference Config,
            Entity entity,
            in AreaTriggerSystem.AreaTriggerInfo triggerInfo)
        {
            if (!pooledEventLookup.HasComponent(entity) || pooledEventLookup.IsComponentEnabled(entity))
            {
                return;
            }

            switch (triggerInfo.TriggerAreaType)
            {
                case TriggerAreaType.FearPointTrigger:
                    {
                        var processScaryRunningComponent = new ProcessScaryRunningTag()
                        {
                            TriggerPosition = triggerInfo.TriggerPosition
                        };

                        commandBuffer.AddComponent(entityInQueryIndex, entity, processScaryRunningComponent);
                        commandBuffer.AddComponent<ScaryRunningTag>(entityInQueryIndex, entity);
                        commandBuffer.SetComponentEnabled<ScaryRunningTag>(entityInQueryIndex, entity, false);

                        break;
                    }
            }

            var id = (int)triggerInfo.TriggerAreaType;

            ref var triggerDataConfigs = ref Config.Config.Value.TriggerDataConfigs;
            var duration = 0f;

            if (triggerDataConfigs.Length > id)
            {
                duration = triggerDataConfigs[id].ImpactTriggerDuration;
            }

            commandBuffer.AddComponent<HasImpactTriggerTag>(entityInQueryIndex, entity);

            if (duration > 0)
            {
                commandBuffer.AddComponent(entityInQueryIndex, entity, new ImpactTriggerData()
                {
                    Duration = duration
                });
            }
        }
    }
}