using Spirit604.AnimationBaker.Entities;
using Spirit604.Attributes;
using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Config;
using Spirit604.DotsCity.Simulation.Factory.Pedestrian;
using Spirit604.DotsCity.Simulation.Npc.Navigation;
using Spirit604.DotsCity.Simulation.Pedestrian;
using Spirit604.DotsCity.Simulation.Pedestrian.Authoring;
using Spirit604.DotsCity.Simulation.Traffic.Authoring;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Initialization
{
    public class PedestrianGlobalInitializer : InitializerBase
    {
        #region Variables

        private World entityWorld;

        #endregion

        #region Constructor

        private GeneralSettingDataSimulation generalSettingData;
        private PedestrianSpawnerConfigHolder pedestrianSpawnerConfigHolder;
        private PedestrianSkinFactory pedestrianSkinFactory;
        private PedestrianCrowdSkinFactory pedestrianBakedSkinFactory;
        private PedestrianRagdollSpawner pedestrianRagdollSpawner;
        private TrafficSettings trafficSettings;

        [InjectWrapper]
        public void Construct(
            GeneralSettingDataSimulation generalSettingData,
            PedestrianSpawnerConfigHolder pedestrianSpawnerConfigHolder,
            PedestrianSkinFactory pedestrianSkinFactory,
            PedestrianCrowdSkinFactory pedestrianBakedSkinFactory,
            PedestrianRagdollSpawner pedestrianRagdollSpawner,
            TrafficSettings trafficSettings)
        {
            this.generalSettingData = generalSettingData;
            this.pedestrianSpawnerConfigHolder = pedestrianSpawnerConfigHolder;
            this.pedestrianSkinFactory = pedestrianSkinFactory;
            this.pedestrianBakedSkinFactory = pedestrianBakedSkinFactory;
            this.pedestrianRagdollSpawner = pedestrianRagdollSpawner;
            this.trafficSettings = trafficSettings;
        }

        #endregion

        #region InitializerBase methods

        public override void Initialize()
        {
            base.Initialize();
            InitializeInternal();
        }

        #endregion

        #region Methods

        private void InitializeInternal()
        {
            entityWorld = World.DefaultGameObjectInjectionWorld;

            InitSystems();
        }

        private void InitSystems()
        {
            var pedestrianSettingsConfig = pedestrianSpawnerConfigHolder.PedestrianSettingsConfig;

            switch (pedestrianSettingsConfig.ObstacleAvoidanceType)
            {
                case ObstacleAvoidanceType.Disabled:
                    {
                        DefaultWorldUtils.SwitchActiveUnmanagedSystem<NpcRecalculateNavTargetSystem>(false);
                        DefaultWorldUtils.SwitchActiveUnmanagedSystem<RevertNavAgentTargetSystem>(false);
                        break;
                    }
                case ObstacleAvoidanceType.CalcNavPath:
                    {
                        if (pedestrianSettingsConfig.PedestrianNavigationType == NpcNavigationType.Persist)
                        {
                            DefaultWorldUtils.SwitchActiveUnmanagedSystem<RevertNavAgentTargetSystem>(false);
                        }

                        var hasNavObstacle = trafficSettings.TrafficSettingsConfig?.HasNavObstacle ?? false;

                        if (!hasNavObstacle)
                        {
                            UnityEngine.Debug.Log("PedestrianGlobalInitializer. ObstacleAvoidanceType.CalcNavPath enabled for pedestrians. Make sure, that NavMesh obstacle for traffic is enabled in the traffic settings");
                        }

                        break;
                    }
                case ObstacleAvoidanceType.LocalAvoidance:
                    {
                        DefaultWorldUtils.SwitchActiveUnmanagedSystem<LocalAvoidanceObstacleSystem>(true);
                        DefaultWorldUtils.SwitchActiveUnmanagedSystem<FollowAvoidanceSystem>(true);
                        break;
                    }
            }

            var pedestrianCollisionType = pedestrianSettingsConfig.PedestrianCollisionType;
            var pedestrianEntityType = pedestrianSettingsConfig.PedestrianEntityType;

            bool hasPhysics = generalSettingData.SimulationType != Unity.Physics.SimulationType.NoPhysics &&
                pedestrianSettingsConfig.PedestrianEntityType == EntityType.Physics;

            if (!hasPhysics)
            {
                if (pedestrianEntityType == EntityType.Physics)
                {
                    pedestrianEntityType = EntityType.NoPhysics;
                }

                if (pedestrianCollisionType == CollisionType.Physics)
                {
                    pedestrianCollisionType = CollisionType.Calculate;
                }
            }

            switch (pedestrianEntityType)
            {
                case EntityType.NoPhysics:
                    break;
                case EntityType.Physics:
                    {
                        DefaultWorldUtils.SwitchActiveUnmanagedSystem<PhysicsCollisionStateSystem>(true);
                        break;
                    }
            };

            switch (pedestrianCollisionType)
            {
                case CollisionType.Calculate:
                    {
                        break;
                    }
                case CollisionType.Physics:
                    {
                        DefaultWorldUtils.SwitchActiveUnmanagedSystem<CalculateCollisionSystem>(false);
                        break;
                    }
                case CollisionType.Disabled:
                    {
                        DefaultWorldUtils.SwitchActiveUnmanagedSystem<CalculateCollisionSystem>(false);
                        entityWorld.GetOrCreateSystemManaged<ReactionCollisionSystem>().Enabled = false;
                        break;
                    }
            }

            var npcSkinType = pedestrianSettingsConfig.PedestrianSkinType;
            var npcRigType = pedestrianSettingsConfig.PedestrianRigType;

            var hasSkin = npcSkinType == NpcSkinType.RigShowOnlyInView || npcSkinType == NpcSkinType.RigShowAlways || npcSkinType == NpcSkinType.RigAndDummy;

            if (hasSkin)
            {
                switch (npcRigType)
                {
                    case NpcRigType.HybridLegacy:
                        {
                            var pedestrianLoadSkinSystem = entityWorld.GetOrCreateSystemManaged<LoadHybridSkinSystem>();
                            pedestrianLoadSkinSystem.Initialize(pedestrianSkinFactory);
                            break;
                        }
                    case NpcRigType.PureGPU:
                        {
                            pedestrianBakedSkinFactory.CreateFactory();

                            var crowdSkinProviderSystem = entityWorld.GetOrCreateSystemManaged<CrowdSkinProviderSystem>();
                            crowdSkinProviderSystem.Initialize(pedestrianBakedSkinFactory);
                            crowdSkinProviderSystem.CreateBlobEntity();
                            break;
                        }

                    case NpcRigType.HybridAndGPU:
                        {
                            var pedestrianLoadSkinSystem = entityWorld.GetOrCreateSystemManaged<LoadHybridGPUSkinSystem>();
                            pedestrianLoadSkinSystem.Initialize(pedestrianSkinFactory);

                            var unloadHybridGPUSkinSystem = entityWorld.GetOrCreateSystemManaged<UnloadHybridGPUSkinSystem>();
                            unloadHybridGPUSkinSystem.Enabled = true;

                            var crowdSkinProviderSystem = entityWorld.GetOrCreateSystemManaged<CrowdSkinProviderSystem>();
                            crowdSkinProviderSystem.Initialize(pedestrianBakedSkinFactory);
                            crowdSkinProviderSystem.CreateBlobEntity();
                            break;
                        }
                }

                var pedestrianRagdollSystem = entityWorld.GetOrCreateSystemManaged<RagdollSystem>();
                pedestrianRagdollSystem.Initialize(pedestrianRagdollSpawner);
            }
        }

        #endregion
    }
}
