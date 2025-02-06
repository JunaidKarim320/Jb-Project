using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Car;
using Spirit604.DotsCity.Simulation.Car.Custom;
using Unity.Burst;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Traffic
{
    [UpdateInGroup(typeof(BeginSimulationGroup))]
    [BurstCompile]
    public partial struct TrafficVehicleCustomInputSystem : ISystem
    {
        private EntityQuery updateQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            updateQuery = SystemAPI.QueryBuilder()
                .WithNone<CarStoppingEngineStartedTag>()
                .WithDisabled<TrafficIdleTag>()
                .WithAll<TrafficTag, HasDriverTag, AliveTag>()
                .Build();

            state.RequireForUpdate(updateQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var customInputJob = new CustomInputJob()
            {
            };

            customInputJob.ScheduleParallel();
        }

        [WithNone(typeof(CarStoppingEngineStartedTag))]
        [WithDisabled(typeof(TrafficIdleTag))]
        [WithAll(typeof(TrafficTag), typeof(HasDriverTag), typeof(AliveTag))]
        [BurstCompile]
        public partial struct CustomInputJob : IJobEntity
        {
            void Execute(
                ref VehicleInputReader trafficInputComponent,
                in CustomSteeringData customSteeringData,
                in TrafficMovementComponent trafficMovementComponent)
            {
                trafficInputComponent.SteeringInput = trafficMovementComponent.DesiredSteeringAngle / customSteeringData.MaxSteeringAngle;
            }
        }
    }
}
