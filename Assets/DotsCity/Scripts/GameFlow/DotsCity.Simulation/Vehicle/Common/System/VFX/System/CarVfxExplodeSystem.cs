using Spirit604.DotsCity.Hybrid.VFX;
using Spirit604.DotsCity.Simulation.Car;
using Spirit604.DotsCity.Simulation.Car.Sound;
using Spirit604.DotsCity.Simulation.Sound.Utils;
using Spirit604.DotsCity.Simulation.VFX;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Spirit604.DotsCity.Simulation.Traffic
{
    [UpdateInGroup(typeof(CarVisualDamageSimulationGroup))]
    public partial class CarVfxExplodeSystem : EndSimulationSystemBase
    {
        private VFXFactory vfxFactory;
        private EntityQuery updateQuery;
        private EntityQuery soundPrefabQuery;

        protected override void OnCreate()
        {
            base.OnCreate();

            soundPrefabQuery = SoundExtension.GetSoundQuery(EntityManager);

            updateQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithNone<CarExplodeVfxProcessedTag>()
                .WithAll<CarExplodeRequestedTag>()
                .Build(this);

            RequireForUpdate(updateQuery);
            Enabled = false;
        }

        protected override void OnUpdate()
        {
            var commandBuffer = GetCommandBuffer();

            var soundConfig = SystemAPI.GetSingleton<CarSoundCommonConfigReference>().Config;
            var soundPrefabEntity = soundPrefabQuery.GetSingletonEntity();

            Entities.WithoutBurst()
            .WithNone<CarExplodeVfxProcessedTag>()
            .WithAll<CarExplodeRequestedTag>()
            .ForEach((
                Entity entity,
                ref CarStartExplodeComponent trafficStartExplodeComponent,
                in LocalTransform transform) =>
            {
                if (trafficStartExplodeComponent.ExplodeIsEnabled == 1 && trafficStartExplodeComponent.VfxIsCreated == 0)
                {
                    trafficStartExplodeComponent.VfxIsCreated = 1;

                    var explosionVFX = vfxFactory.GetVFX(VFXType.DefaultCarExplosion);
                    var smokeVFX = vfxFactory.GetVFX(VFXType.DefaultCarSmoke);

                    explosionVFX.GetComponent<VFXBehaviour>().Play(transform.Position);
                    smokeVFX.GetComponent<VFXBehaviour>().Play(entity, TrafficPoolSystem.MAX_EXPLODE_DURATION_TIME - 0.1f, new float3(0, 2f, 0));

                    commandBuffer.AddComponent(entity, new CarExplodeVfxProcessedTag());
                    commandBuffer.CreateSoundEntity(soundPrefabEntity, soundConfig.Value.CarExplodeSoundId, transform.Position);
                }
            }).Run();

            AddCommandBufferForProducer();
        }

        public void Initialize(VFXFactory vfxFactory)
        {
            this.vfxFactory = vfxFactory;
            Enabled = true;
        }
    }
}