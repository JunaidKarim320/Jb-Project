using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Core.Initialization;
using Spirit604.DotsCity.Gameplay.Bootstrap;
using Spirit604.DotsCity.Gameplay.Car;
using Spirit604.DotsCity.Gameplay.Config.Common;
using Spirit604.DotsCity.Gameplay.Npc;
using Spirit604.DotsCity.Gameplay.Player;
using Spirit604.DotsCity.Gameplay.Traffic;
using Spirit604.DotsCity.Gameplay.Weapon;
using Spirit604.DotsCity.Simulation.Config;
using Spirit604.DotsCity.Simulation.Initialization;
using Spirit604.DotsCity.Simulation.Npc;
using Spirit604.DotsCity.Simulation.Pedestrian;
using Spirit604.DotsCity.Simulation.Root.Authoring;
using Spirit604.DotsCity.Simulation.Traffic;
using Spirit604.Extensions;
using Spirit604.Gameplay.InputService;
using Unity.Entities;
using UnityEngine;
using static Spirit604.DotsCity.Simulation.Initialization.CitySettingsSimulationInitializer;

namespace Spirit604.DotsCity.Gameplay.Initialization
{
    public class CitySettingsInitializer : CitySettingsInitializerBase<GeneralSettingData>, ISyncableConfig, IConfigInject
    {
        [SerializeField] private InputSettingsProvider inputSettingsProvider;

        private IInputSettings InputSettings => inputSettingsProvider.GetInputSettings();

#if UNITY_EDITOR
        private void OnEnable()
        {
            SceneBootstrap.OnEntityLoaded += SceneBootstrap_OnEntityLoaded;
        }

        private void OnDisable()
        {
            SceneBootstrap.OnEntityLoaded -= SceneBootstrap_OnEntityLoaded;
        }
#endif
        public void InjectConfig(object config)
        {
            Settings = config as GeneralSettingData;
        }

        public override void Initialize()
        {
            base.Initialize();

            CitySettingsSimulationInitializer.InitializeStatic(this, Settings);
            CitySettingsCoreInitializer.InitializeStatic(Settings);

            var world = World.DefaultGameObjectInjectionWorld;

            if (world == null)
                return;

            if (!Settings)
                return;

            if (!Settings.BulletSupport)
            {
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<BulletLifeTimeSystem>(false);
                world.GetOrCreateSystemManaged<BulletMovementSystem>().Enabled = false;
                world.GetOrCreateSystemManaged<NpcBulletCollisionSystem>().Enabled = false;
                world.GetOrCreateSystemManaged<TrafficBulletCollisionSystem>().Enabled = false;
            }
            else
            {
                switch (Settings.BulletType)
                {
                    case GeneralSettingData.BulletCollisionType.CalculateCollision:
                        world.GetOrCreateSystemManaged<BulletRaycastSystem>().Enabled = false;
                        break;
                    case GeneralSettingData.BulletCollisionType.Raycast:
                        world.GetOrCreateSystemManaged<NpcBulletCollisionSystem>().Enabled = false;
                        world.GetOrCreateSystemManaged<TrafficBulletCollisionSystem>().Enabled = false;
                        world.GetOrCreateSystemManaged<BulletHashMapSystem>().Enabled = false;
                        break;
                }
            }

            if (!Settings.HealthSystemSupport)
            {
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<CarHealthSystem>(false);
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<CarImpulseExplodeSystem>(false);
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<CarImpulseExplodeNoPhysicsSystem>(false);
                world.GetOrCreateSystemManaged<CarVfxExplodeSystem>().Enabled = false;
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<HealthWithRagdollSystem>(false);
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<HealthNoRagdollSystem>(false);
                world.GetOrCreateSystemManaged<RagdollSystem>().Enabled = false;
                world.GetOrCreateSystemManaged<NpcRagdollSystem>().Enabled = false;
                world.GetOrCreateSystemManaged<NpcProcessHitReactionSystem>().Enabled = false;
            }
        }

        public void SyncConfig()
        {
            var hubParent = ObjectUtils.FindObjectOfType<EntityRootSubsceneGenerator>();

            if (hubParent)
            {
                var citySettingsInitializer = hubParent.GetComponentInChildren<CitySettingsInitializer>();

                if (citySettingsInitializer && citySettingsInitializer != this)
                {
                    this.Settings = citySettingsInitializer.Settings;
                    EditorSaver.SetObjectDirty(this);
                }
            }
        }

        public class GeneralSettingsBaker : Baker<CitySettingsInitializer>
        {
            public override void Bake(CitySettingsInitializer authoring)
            {
                DependsOn(authoring.Settings);

                GeneralSettingsSimulationBaker.Bake(this, authoring.Settings);

                var playerTargetSettingsDataEntity = CreateAdditionalEntity(TransformUsageFlags.None);

                AddComponent(playerTargetSettingsDataEntity, PlayerTargetSettingsRuntimeAuthoring.CreateConfigStatic(this, authoring.Settings.GetPlayerTargetSettings(authoring.InputSettings)));
            }
        }

#if UNITY_EDITOR

        private void SceneBootstrap_OnEntityLoaded()
        {
            var SettingsQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(CommonGeneralSettingsReference));

            if (SettingsQuery.CalculateEntityCount() > 0)
            {
            }
            else
            {
                UnityEngine.Debug.Log("CitySettingsInitializer. CommonGeneralSettingsReference config not found");
            }
        }

#endif
    }
}