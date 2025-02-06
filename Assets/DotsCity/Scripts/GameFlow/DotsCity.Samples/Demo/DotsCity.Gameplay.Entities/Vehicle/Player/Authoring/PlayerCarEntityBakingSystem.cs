using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Hybrid.Core;
using Spirit604.DotsCity.Simulation.Car;
using Spirit604.DotsCity.Simulation.Traffic;
using Spirit604.DotsCity.Simulation.Traffic.Authoring;
using Unity.Collections;
using Unity.Entities;
using static Spirit604.DotsCity.Gameplay.Player.Authoring.PlayerCarEntityAuthoring;

namespace Spirit604.DotsCity.Gameplay.Player.Authoring
{
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    [UpdateInGroup(typeof(PostBakingSystemGroup))]
    public partial class PlayerCarEntityBakingSystem : SystemBase
    {
        private EntityQuery bakingCarQuery;

        protected override void OnCreate()
        {
            base.OnCreate();

            bakingCarQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PlayerCarEntityBakingTag>()
                .WithOptions(EntityQueryOptions.IncludePrefab)
                .Build(this);

            RequireForUpdate(bakingCarQuery);
            RequireForUpdate<RaycastConfigReference>();
            RequireForUpdate<TrafficCommonSettingsConfigBlobReference>();
        }

        protected override void OnUpdate()
        {
            var raycastConfigReference = SystemAPI.GetSingleton<RaycastConfigReference>();
            var trafficCommonSettingsConfigBlobReference = SystemAPI.GetSingleton<TrafficCommonSettingsConfigBlobReference>();
            var generalCoreSettingsDataReference = SystemAPI.GetSingleton<GeneralCoreSettingsDataReference>();
            var entityManager = EntityManager;
            var commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

            Entities
            .WithoutBurst()
            .WithEntityQueryOptions(EntityQueryOptions.IncludePrefab)
            .WithAll<PlayerCarEntityBakingTag>()
            .ForEach((
                Entity prefabEntity) =>
            {
                commandBuffer.AddComponent(prefabEntity, typeof(VehicleInputReader));

                switch (generalCoreSettingsDataReference.Config.Value.WorldSimulationType)
                {
                    case WorldSimulationType.DOTS:
                        commandBuffer.AddComponent(prefabEntity, typeof(InterpolateTransformData));
                        break;
                    case WorldSimulationType.HybridMono:
                        commandBuffer.AddComponent(prefabEntity, typeof(CopyTransformFromGameObject));
                        break;
                }

                if (generalCoreSettingsDataReference.Config.Value.DOTSSimulation)
                {
                    if (trafficCommonSettingsConfigBlobReference.Reference.Value.DetectObstacleMode != DetectObstacleMode.CalculateOnly)
                    {
                        TrafficEntityBakingUtils.CheckForPhysicsLayer(entityManager, prefabEntity, in raycastConfigReference, "PlayerCarEntityBakingSystem", "player car");
                    }
                }

            }).Run();

            commandBuffer.Playback(entityManager);
            commandBuffer.Dispose();
        }
    }
}