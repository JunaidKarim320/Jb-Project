#if UNITY_EDITOR
using Spirit604.DotsCity.Core;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Traffic
{
    [UpdateInGroup(typeof(TrafficSimulationGroup))]
    public partial class TrafficCleanerSystem : EndSimulationSystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            Enabled = false;
        }

        protected override void OnUpdate()
        {
            var commandBuffer = GetParallelCommandBuffer();

            var job = Entities
                .WithBurst()
                .WithNone<PooledEventTag>()
                .WithAll<TrafficTag, PoolableTag>()
                .ForEach((
                    Entity entity,
                    int entityInQueryIndex) =>
                {
                    PoolEntityUtils.DestroyEntity(ref commandBuffer, entityInQueryIndex, entity);
                }).ScheduleParallel(this.Dependency);

            AddCommandBufferForProducer();

            job.Complete();

            Enabled = false;
        }

        public void Clear()
        {
            Enabled = true;
        }
    }
}
#endif