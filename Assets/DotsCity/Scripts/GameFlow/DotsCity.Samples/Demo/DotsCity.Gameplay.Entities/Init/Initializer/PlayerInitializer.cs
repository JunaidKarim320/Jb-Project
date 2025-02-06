using Spirit604.Attributes;
using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Gameplay.Config.Common;
using Spirit604.DotsCity.Gameplay.Factory.Player;
using Spirit604.DotsCity.Gameplay.Player;
using Spirit604.DotsCity.Gameplay.Player.Session;
using Spirit604.DotsCity.Gameplay.UI;
using Spirit604.DotsCity.Simulation.Car;
using Spirit604.Gameplay.InputService;
using Spirit604.Gameplay.Npc;
using Unity.Entities;

namespace Spirit604.DotsCity.Gameplay.Initialization
{
    public class PlayerInitializer : InitializerBase
    {
        private GeneralSettingData generalSettingData;
        private IPlayerInteractCarService playerInteractCarService;
        private ICarConverter carConverter;
        private IPlayerEntityTriggerProccesor playerEntityTriggerProccesor;
        private PlayerActorTracker playerTargetHandler;
        private PlayerCarPool playerCarPool;
        private PlayerEnterCarStatePresenter playerEnterCarStatePresenter;
        private IMotionInput motionInput;
        private ICarMotionInput carMotionInput;
        private IShootTargetProvider targetProvider;
        private PlayerSession playerSession;

        [InjectWrapper]
        public void Construct(
            GeneralSettingData generalSettingData,
            IPlayerInteractCarService playerInteractCarService,
            ICarConverter carConverter,
            IPlayerEntityTriggerProccesor playerEntityTriggerProccesor,
            PlayerActorTracker playerTargetHandler,
            PlayerCarPool playerCarPool,
            PlayerEnterCarStatePresenter playerEnterCarStatePresenter,
            IMotionInput motionInput,
            ICarMotionInput carMotionInput,
            IShootTargetProvider targetProvider,
            PlayerSession playerSession)
        {
            this.generalSettingData = generalSettingData;
            this.playerInteractCarService = playerInteractCarService;
            this.carConverter = carConverter;
            this.playerEntityTriggerProccesor = playerEntityTriggerProccesor;
            this.playerTargetHandler = playerTargetHandler;
            this.playerCarPool = playerCarPool;
            this.playerEnterCarStatePresenter = playerEnterCarStatePresenter;
            this.motionInput = motionInput;
            this.carMotionInput = carMotionInput;
            this.targetProvider = targetProvider;
            this.playerSession = playerSession;
        }

        public override void Initialize()
        {
            base.Initialize();

            var world = World.DefaultGameObjectInjectionWorld;

            if (generalSettingData.BuiltInInteraction)
            {
                var playerInteractCarSystem = world.GetOrCreateSystemManaged<PlayerInteractCarSystem>();

                playerInteractCarSystem.Initialize(playerInteractCarService, carConverter);
                playerInteractCarSystem.Enabled = generalSettingData.PlayerSelected;
                world.GetOrCreateSystemManaged<PlayerInteractCarStateListenerSystem>().Initialize(playerEnterCarStatePresenter);

                DefaultWorldUtils.SwitchActiveUnmanagedSystem<PlayerGetAvailableCarForEnterSystem>(true);
            }
            else
            {
                playerEnterCarStatePresenter.SwitchPlayerInteractState(PlayerInteractCarState.OutOfCar);
            }

            if (generalSettingData.BuiltInSolution)
            {
                if (generalSettingData.DOTSSimulation)
                {
                    var playerEnterTriggerSystem = world.GetOrCreateSystemManaged<PlayerEnterTriggerSystem>();

                    playerEnterTriggerSystem.Initialize(playerEntityTriggerProccesor);
                    playerEnterTriggerSystem.Enabled = generalSettingData.PlayerSelected;

                    world.GetOrCreateSystemManaged<MobEnterCarSystem>().Initialize(playerTargetHandler);
                    world.GetOrCreateSystemManaged<PlayerInputSystem>().Initialize(motionInput, targetProvider);
                }

                world.GetOrCreateSystemManaged<PlayerVehicleInputSystem>().Initialize(carMotionInput);
            }

            if (generalSettingData.PedestrianTriggerSystemSupport)
            {
                world.GetOrCreateSystemManaged<PlayerScaryTriggerSystem>().Initialize(playerSession);
            }

            playerCarPool.Initialize();
        }
    }
}
