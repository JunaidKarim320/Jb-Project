using Spirit604.Attributes;
using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Core.Initialization;
using Spirit604.DotsCity.NavMesh;
using Spirit604.DotsCity.Simulation.Car;
using Spirit604.DotsCity.Simulation.Config;
using Spirit604.DotsCity.Simulation.Factory.Traffic;
using Spirit604.DotsCity.Simulation.Traffic;
using Spirit604.DotsCity.Simulation.Traffic.Authoring;
using Unity.Entities;
using Unity.Physics;

namespace Spirit604.DotsCity.Simulation.Initialization
{
    public class TrafficInitializer : InitializerBase
    {
        private World world;

        private GeneralSettingDataSimulation generalSettings;
        private TrafficCarPoolGlobal trafficPoolGlobal;
        private TrafficSettings trafficSettings;
        private NavMeshObstacleFactory navMeshObstacleFactory;

        [InjectWrapper]
        public void Construct(
            GeneralSettingDataSimulation generalSettings,
            TrafficCarPoolGlobal trafficPoolGlobal,
            TrafficSettings trafficSettings,
            NavMeshObstacleFactory navMeshObstacleFactory)
        {
            this.generalSettings = generalSettings;
            this.trafficPoolGlobal = trafficPoolGlobal;
            this.trafficSettings = trafficSettings;
            this.navMeshObstacleFactory = navMeshObstacleFactory;
        }

        public override void Initialize()
        {
            InitializeInternal();
            InitSystems();
        }

        private void InitializeInternal()
        {
            world = World.DefaultGameObjectInjectionWorld;

            var nasNavObstacle = generalSettings.NavigationSupport && trafficSettings.TrafficSettingsConfig.HasNavObstacle;

            world.GetOrCreateSystemManaged<NavMeshObstacleLoader>().Enabled = nasNavObstacle;

            InitHitReaction();

            world.GetOrCreateSystemManaged<NavMeshObstacleLoader>().Initialize(navMeshObstacleFactory);

            var trafficSpawnerSystem = world.GetOrCreateSystemManaged<TrafficSpawnerSystem>();
            trafficSpawnerSystem.Initialize(generalSettings, trafficSettings);
        }

        private void InitHitReaction()
        {
            var entityType = trafficSettings.EntityType;
            var poolData = trafficPoolGlobal.GetPoolData(entityType);

            if (poolData == null)
            {
                UnityEngine.Debug.Log($"TrafficInitializer. PoolData {entityType} is null");
                return;
            }

            if (generalSettings.CarVisualDamageSystemSupport && generalSettings.DOTSSimulation)
            {
                var carHitReactProviderSystem = world.GetOrCreateSystemManaged<CarHitReactProviderSystem>();

                var query = carHitReactProviderSystem.GetPrefabQuery();

                var initAwaiter = new SystemInitAwaiter(this, () => query.CalculateEntityCount() == 0,
                    () =>
                    {
                        carHitReactProviderSystem.Initialize();
                    });

                initAwaiter.StartInit();
            }
        }

        private void InitSystems()
        {
            if (trafficSettings.TrafficSettingsConfig.HybridEntity)
            {
                var trafficInitializeHybridHullSystem = world.GetOrCreateSystemManaged<TrafficInitializeHybridHullSystem>();
                trafficInitializeHybridHullSystem.Initialize(trafficPoolGlobal);
            }

            switch (trafficSettings.TrafficSettingsConfig.DetectObstacleMode)
            {
                case DetectObstacleMode.Hybrid:
                    break;
                case DetectObstacleMode.CalculateOnly:
                    break;
                case DetectObstacleMode.RaycastOnly:
                    break;
            }

            var hasLerp = trafficSettings.TrafficSettingsConfig.HasRotationLerp;

            if (hasLerp)
            {
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficSpeedRotationSystem>(true);
            }

            var currentTrafficSettings = trafficSettings.TrafficSettingsConfig;

            var trafficDetectObstacleMode = currentTrafficSettings.DetectObstacleMode;
            var trafficDetectNpcMode = currentTrafficSettings.DetectNpcMode;

            switch (trafficDetectNpcMode)
            {
                case DetectNpcMode.Disabled:
                    {
                        DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficNpcCalculateObstacleSystem>(false);
                        break;
                    }
                case DetectNpcMode.Calculate:
                    break;
                case DetectNpcMode.Raycast:
                    {
                        DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficNpcCalculateObstacleSystem>(false);
                        break;
                    }
            }

            bool hasRaycast = HasRaycast(currentTrafficSettings);

            if (hasRaycast)
            {
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficEnableRaycastSystem>(true);

                if (generalSettings.DOTSSimulation)
                {
                    DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficRaycastObstacleSystem>(true);
                }
                else
                {
                    DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficMonoRaycastObstacleSystem>(true);
                }
            }
            else
            {
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficRaycastEnableCustomTargetSelectorSystem>(false);
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficRaycastDisableCustomTargetSelectorSystem1>(false);
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficRaycastDisableCustomTargetSelectorSystem2>(false);
            }

            if (generalSettings.SimulationType == SimulationType.NoPhysics || currentTrafficSettings.NoPhysicsEntity)
            {
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficCollisionEventSystem>(false);
            }
        }

        public static bool HasRaycast(TrafficCarSettingsConfig currentTrafficSettings)
        {
            return currentTrafficSettings.DetectObstacleMode == DetectObstacleMode.Hybrid || currentTrafficSettings.DetectObstacleMode == DetectObstacleMode.RaycastOnly || currentTrafficSettings.DetectNpcMode == DetectNpcMode.Raycast;
        }
    }
}
