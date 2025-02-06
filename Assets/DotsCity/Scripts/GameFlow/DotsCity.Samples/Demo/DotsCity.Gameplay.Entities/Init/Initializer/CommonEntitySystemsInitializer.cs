using Spirit604.Attributes;
using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Gameplay.CameraService;
using Spirit604.DotsCity.Gameplay.Config.Common;
using Spirit604.DotsCity.Gameplay.Events;
using Spirit604.DotsCity.Gameplay.Npc;
using Spirit604.DotsCity.Simulation.Car;
using Spirit604.DotsCity.Simulation.Pedestrian.Authoring;
using Spirit604.DotsCity.Simulation.Traffic;
using Spirit604.DotsCity.Simulation.VFX;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Gameplay.Initialization
{
    public class CommonEntitySystemsInitializer : InitializerBase
    {
        private VFXFactory vfxFactory;
        private GeneralSettingData generalSettingData;
        private PedestrianSpawnerConfigHolder pedestrianSpawnerConfigHolder;
        private CameraController cameraController;

        [InjectWrapper]
        public void Construct(
            VFXFactory vfxFactory,
            GeneralSettingData generalSettingData,
            PedestrianSpawnerConfigHolder pedestrianSpawnerConfigHolder,
            CameraController cameraController = null)
        {
            this.vfxFactory = vfxFactory;
            this.generalSettingData = generalSettingData;
            this.pedestrianSpawnerConfigHolder = pedestrianSpawnerConfigHolder;
            this.cameraController = cameraController;
        }

        public override void Initialize()
        {
            var world = World.DefaultGameObjectInjectionWorld;

            // If the user wants to replace the camera with his own solution.
            if (cameraController)
                world.GetOrCreateSystemManaged<CameraShakeEventSystem>().Initialize(cameraController);
       
            if (generalSettingData.DOTSSimulation)
            {
                world.GetOrCreateSystemManaged<CarVfxExplodeSystem>().Initialize(vfxFactory);

                if (generalSettingData.CarVisualDamageSystemSupport)
                {
                    world.GetOrCreateSystemManaged<BulletHitReactionSystem>().Initialize(vfxFactory);
                }

                if (generalSettingData.SimulationType != Unity.Physics.SimulationType.NoPhysics)
                {
                    DefaultWorldUtils.SwitchActiveUnmanagedSystem<NpcGroundStateSystem>(true);
                }

                var pedestrianSettings = pedestrianSpawnerConfigHolder.PedestrianSettingsConfig;

                var hasLegacyPhysics = false;

                if (generalSettingData.SimulationType != Unity.Physics.SimulationType.NoPhysics || generalSettingData.ForceLegacyPhysics)
                {
                    hasLegacyPhysics = generalSettingData.ForceLegacyPhysics || pedestrianSettings.HasRagdoll;
                }

                Physics.simulationMode = hasLegacyPhysics ? SimulationMode.FixedUpdate : SimulationMode.Script;
            }
        }
    }
}