using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Car;
using Spirit604.DotsCity.Simulation.Traffic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Spirit604.DotsCity.Simulation.Train
{
    [UpdateAfter(typeof(TrafficApproachSystem))]
    [UpdateInGroup(typeof(TrafficSimulationGroup))]
    public partial struct TrafficWagonSyncSystem : ISystem
    {
        private EntityQuery updateQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            updateQuery = SystemAPI.QueryBuilder()
                .WithAll<TrafficWagonComponent>()
                .Build();

            state.RequireForUpdate(updateQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var syncJob = new SyncJob()
            {
                TrafficApproachDataComponentLookup = SystemAPI.GetComponentLookup<TrafficApproachDataComponent>(false),
                TrafficObstacleComponentLookup = SystemAPI.GetComponentLookup<TrafficObstacleComponent>(false),
                VehicleInputReaderLookup = SystemAPI.GetComponentLookup<VehicleInputReader>(false),
                SpeedComponentLookup = SystemAPI.GetComponentLookup<SpeedComponent>(false),
                BoundsComponentLookup = SystemAPI.GetComponentLookup<BoundsComponent>(true),
                LocalTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true),
                TrainDataComponentLookup = SystemAPI.GetComponentLookup<TrainDataComponent>(true),
                TrafficRailConfigReference = SystemAPI.GetSingleton<TrafficRailConfigReference>(),
            };

            syncJob.Schedule();
        }

        [BurstCompile]
        partial struct SyncJob : IJobEntity
        {
            public ComponentLookup<TrafficApproachDataComponent> TrafficApproachDataComponentLookup;
            public ComponentLookup<TrafficObstacleComponent> TrafficObstacleComponentLookup;
            public ComponentLookup<VehicleInputReader> VehicleInputReaderLookup;
            public ComponentLookup<SpeedComponent> SpeedComponentLookup;

            [ReadOnly]
            public ComponentLookup<BoundsComponent> BoundsComponentLookup;

            [ReadOnly]
            public ComponentLookup<LocalTransform> LocalTransformLookup;

            [ReadOnly]
            public ComponentLookup<TrainDataComponent> TrainDataComponentLookup;

            [ReadOnly]
            public TrafficRailConfigReference TrafficRailConfigReference;

            void Execute(
                Entity entity,
                ref VelocityComponent velocityComponent,
                in LocalTransform localTransform,
                in TrainComponent trainComponent,
                in TrafficWagonComponent trafficWagonComponent)
            {
                TrafficApproachDataComponentLookup[entity] = TrafficApproachDataComponentLookup[trafficWagonComponent.OwnerEntity];
                TrafficObstacleComponentLookup[entity] = TrafficObstacleComponentLookup[trafficWagonComponent.OwnerEntity];

                var speedComponent = SpeedComponentLookup[trafficWagonComponent.OwnerEntity];

                float speed = speedComponent.Value;

                var transform = LocalTransformLookup[entity];

                if (trainComponent.NextEntity != Entity.Null)
                {
                    var trainDataComponent = TrainDataComponentLookup[trafficWagonComponent.OwnerEntity];
                    var targetTranform = LocalTransformLookup[trainComponent.NextEntity];

                    float distance = math.distance(targetTranform.Position, transform.Position);
                    var boundsComponent = BoundsComponentLookup[trainComponent.NextEntity];

                    distance -= boundsComponent.Size.z;

                    var diff = 1 + (distance - trainDataComponent.WagonOffset) / trainDataComponent.WagonOffset;

                    diff = math.clamp(diff, TrafficRailConfigReference.Config.Value.ConvergenceSpeedRate.x, TrafficRailConfigReference.Config.Value.ConvergenceSpeedRate.y);
                    speed *= diff;
                }

                speedComponent.Value = speed;
                SpeedComponentLookup[entity] = speedComponent;

                VehicleInputReaderLookup[entity] = VehicleInputReaderLookup[trafficWagonComponent.OwnerEntity];
                velocityComponent.Value = SpeedComponentLookup[entity].Value * localTransform.Forward();
            }
        }
    }
}