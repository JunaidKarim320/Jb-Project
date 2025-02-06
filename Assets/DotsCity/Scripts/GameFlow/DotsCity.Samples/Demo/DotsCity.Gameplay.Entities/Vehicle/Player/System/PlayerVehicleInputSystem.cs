using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Car;
using Spirit604.Gameplay.InputService;
using Unity.Entities;
using Unity.Transforms;

namespace Spirit604.DotsCity.Gameplay.Player
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(PlayerSimulationGroup))]
    public partial class PlayerVehicleInputSystem : SystemBase
    {
        private ICarMotionInput input;

        protected override void OnCreate()
        {
            base.OnCreate();
            Enabled = false;
        }

        protected override void OnUpdate()
        {
            Entities
            .WithName("PlayerVehicleInputJob")
            .WithoutBurst()
            .WithNone<CarStoppingEngineStartedTag>()
            .WithAll<PlayerTag, CarEngineStartedTag, HasDriverTag>()
            .ForEach((ref VehicleInputReader playerVehicleInput, in LocalTransform transform) =>
            {
                var movingInput = input.GetMovementInput(transform.Forward());

                playerVehicleInput.Throttle = movingInput.y;
                playerVehicleInput.SteeringInput = movingInput.x;
                playerVehicleInput.HandbrakeInput = input.Brake ? 1 : 0;
            }).Run();

            Entities
            .WithBurst()
            .WithNone<CarEngineStartedTag>()
            .WithAll<PlayerTag>()
            .ForEach((ref VehicleInputReader playerVehicleInput) =>
            {
                playerVehicleInput = VehicleInputReader.GetBrake();
            }).Schedule();
        }

        public void Initialize(ICarMotionInput input)
        {
            this.input = input;
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PlayerVehicleDummyInputSystem>().Enabled = false;
            Enabled = true;
        }
    }
}