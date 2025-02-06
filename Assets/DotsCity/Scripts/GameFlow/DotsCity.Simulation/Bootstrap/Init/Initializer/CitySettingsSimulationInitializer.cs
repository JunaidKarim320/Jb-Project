using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Core.Initialization;
using Spirit604.DotsCity.Simulation.Config;
using Spirit604.DotsCity.Simulation.Level.Streaming;
using Spirit604.DotsCity.Simulation.Npc.Navigation;
using Spirit604.DotsCity.Simulation.Pedestrian;
using Spirit604.DotsCity.Simulation.Traffic;
using Spirit604.DotsCity.Simulation.TrafficPublic;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Initialization
{
    public class CitySettingsSimulationInitializer : CitySettingsInitializerBase<GeneralSettingDataSimulation>
    {
        public override void Initialize()
        {
            base.Initialize();

            CitySettingsCoreInitializer.InitializeStatic(Settings);
            InitializeStatic(this, Settings);
        }

        public static void InitializeStatic(MonoBehaviour sender, GeneralSettingDataSimulation settings)
        {
            var world = World.DefaultGameObjectInjectionWorld;

            if (!settings.CarVisualDamageSystemSupport)
            {
                world.GetOrCreateSystemManaged<CarVisualDamageSimulationGroup>().Enabled = false;
            }

            if (!settings.HasTraffic)
            {
                world.GetOrCreateSystemManaged<TrafficSpawnerSystem>().Enabled = false;
                world.GetOrCreateSystemManaged<TrafficSpawnerSystem>().ForceDisable = true;
            }

            if (!settings.ChangeLaneSupport)
            {
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficChangeLaneSystem>(false);
            }

            if (!settings.TrafficPublicSupport)
            {
                world.GetOrCreateSystemManaged<TrafficPublicSpawnerSystem>().Enabled = false;
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficPublicNodeAvailableSystem>(false);
            }
            else
            {
                world.GetOrCreateSystemManaged<TrafficPublicSpawnerSystem>().Enabled = false;
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficPublicNodeAvailableSystem>(false);

                var systemAwaiter = new SystemInitAwaiter(sender, () => !TrafficNodeResolverSystem.PathDataHashMapStaticRef.IsCreated, () =>
                {
                    world.GetOrCreateSystemManaged<TrafficPublicSpawnerSystem>().Enabled = true;
                    DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficPublicNodeAvailableSystem>(true);
                });

                systemAwaiter.StartInit();
            }

            if (!settings.AntiStuckSupport)
            {
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficCullStuckedSystem>(false);
            }

            if (!settings.WheelSystemSupport)
            {
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<CarSimpleWheelSystem>(false);
            }

            if (!settings.CarHitCollisionReaction)
            {
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficCollisionEventSystem>(false);
            }

            if (!settings.HasPedestrian)
            {
                world.GetOrCreateSystemManaged<PedestrianEntitySpawnerSystem>().Enabled = false;
                world.GetOrCreateSystemManaged<PedestrianEntitySpawnerSystem>().ForceDisable = true;
                world.GetOrCreateSystemManaged<PedestrianSpawnTalkAreaSystem>().Enabled = false;
                world.GetOrCreateSystemManaged<PedestrianSpawnTalkAreaSystem>().ForceDisable = true;
            }

            if (!settings.PedestrianTriggerSystemSupport)
            {
                world.GetOrCreateSystemManaged<NpcDeathEventConsumerSystem>().Enabled = false;
                world.GetOrCreateSystemManaged<RagdollSystem>().TriggerSupported = false;
            }

            if (!settings.NavigationSupport)
            {
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<NpcRecalculateNavTargetSystem>(false);
            }
        }

        public class GeneralSettingsSimulationBaker : Baker<CitySettingsSimulationInitializer>
        {
            public override void Bake(CitySettingsSimulationInitializer authoring)
            {
                DependsOn(authoring.Settings);
                Bake(this, authoring.Settings);
            }

            public static void Bake(IBaker baker, GeneralSettingDataSimulation settings)
            {
                var coreSettingsEntity = baker.CreateAdditionalEntity(TransformUsageFlags.None);

                baker.AddComponent(coreSettingsEntity, GeneralCoreSettingsRuntimeAuthoring.CreateConfigStatic(baker, settings));

                var commonGeneralSettingsDataEntity = baker.CreateAdditionalEntity(TransformUsageFlags.None);

                baker.AddComponent(commonGeneralSettingsDataEntity, GeneralSettingsCommonRuntimeAuthoring.CreateConfigStatic(baker, settings));

                var pedestrianGeneralSettingsDataEntity = baker.CreateAdditionalEntity(TransformUsageFlags.None);

                baker.AddComponent(pedestrianGeneralSettingsDataEntity, PedestrianGeneralSettingsRuntimeAuthoring.CreateConfigStatic(baker, settings));

                var trafficGeneralSettingsDataEntity = baker.CreateAdditionalEntity(TransformUsageFlags.None);

                baker.AddComponent(trafficGeneralSettingsDataEntity, TrafficGeneralSettingsRuntimeAuthoring.CreateConfigStatic(baker, settings));
            }
        }
    }
}