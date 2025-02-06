using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Car;
using Spirit604.DotsCity.Simulation.Pedestrian;
using Spirit604.DotsCity.Simulation.Pedestrian.Authoring;
using Spirit604.DotsCity.Simulation.Sound.Pedestrian;
using Spirit604.DotsCity.Simulation.Sound.Utils;
using Spirit604.DotsCity.Simulation.Traffic;
using Spirit604.Extensions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Spirit604.DotsCity.Simulation.TrafficPublic
{
    [UpdateInGroup(typeof(TrafficSimulationGroup))]
    [BurstCompile]
    public partial struct TrafficPublicExitPedestrianSystem : ISystem
    {
        private EntityQuery updateQuery;
        private EntityQuery soundPrefabQuery;

        void ISystem.OnCreate(ref SystemState state)
        {
            soundPrefabQuery = SoundExtension.GetSoundQuery(state.EntityManager);

            updateQuery = SystemAPI.QueryBuilder()
                .WithDisabledRW<TrafficPublicExitCompleteTag>()
                .WithAll<TrafficPublicProccessExitTag, AliveTag>()
                .Build();

            state.RequireForUpdate(updateQuery);
            state.RequireForUpdate<PedestrianSettingsReference>();
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var trafficPublicExitPedestrianJob = new TrafficPublicExitPedestrianJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
                ConnectedPedestrianNodeElementLookup = SystemAPI.GetBufferLookup<ConnectedPedestrianNodeElement>(true),
                WorldTransformLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true),
                TrafficDestinationComponentLookup = SystemAPI.GetComponentLookup<TrafficDestinationComponent>(true),
                TrafficWagonComponentLookup = SystemAPI.GetComponentLookup<TrafficWagonComponent>(true),
                NodeConnectionDataLookup = SystemAPI.GetBufferLookup<NodeConnectionDataElement>(true),
                PedestrianSettingsReference = SystemAPI.GetSingleton<PedestrianSettingsReference>(),
                EntityPrefab = SystemAPI.GetSingleton<PedestrianEntityPrefabComponent>().PrefabEntity,
                Time = (float)SystemAPI.Time.ElapsedTime,
                SoundConfigReference = SystemAPI.GetSingleton<SoundConfigReference>(),
                SoundPrefabEntity = soundPrefabQuery.GetSingletonEntity(),
            };

            trafficPublicExitPedestrianJob.Schedule();
        }

        [WithDisabled(typeof(TrafficPublicExitCompleteTag))]
        [WithAll(typeof(AliveTag))]
        [BurstCompile]
        public partial struct TrafficPublicExitPedestrianJob : IJobEntity
        {
            public EntityCommandBuffer CommandBuffer;

            [ReadOnly]
            public BufferLookup<ConnectedPedestrianNodeElement> ConnectedPedestrianNodeElementLookup;

            [ReadOnly]
            public ComponentLookup<LocalToWorld> WorldTransformLookup;

            [ReadOnly]
            public ComponentLookup<TrafficDestinationComponent> TrafficDestinationComponentLookup;

            [ReadOnly]
            public ComponentLookup<TrafficWagonComponent> TrafficWagonComponentLookup;

            [ReadOnly]
            public BufferLookup<NodeConnectionDataElement> NodeConnectionDataLookup;

            [ReadOnly]
            public PedestrianSettingsReference PedestrianSettingsReference;

            [ReadOnly]
            public Entity EntityPrefab;

            [ReadOnly]
            public float Time;

            [ReadOnly]
            public SoundConfigReference SoundConfigReference;

            [ReadOnly]
            public Entity SoundPrefabEntity;

            void Execute(
                Entity entity,
                ref TrafficPublicExitSettingsComponent trafficPublicExitSettingsComponent,
                ref CarCapacityComponent carCapacityComponent,
                EnabledRefRW<TrafficPublicExitCompleteTag> trafficPublicExitCompleteTagRW,
                EnabledRefRW<TrafficPublicProccessExitTag> trafficPublicProccessExitTagRW,
                in DynamicBuffer<VehicleEntryElement> vehicleEntryBuffer)
            {
                var availableForSpawn = (trafficPublicExitSettingsComponent.LastExitTimestamp - Time) <= 0;

                if (!availableForSpawn)
                    return;

                bool init = trafficPublicExitSettingsComponent.LastExitTimestamp == 0;

                var duration = UnityMathematicsExtension.GetRandomValue(trafficPublicExitSettingsComponent.EnterExitDelayDuration, Time, entity.Index);

                trafficPublicExitSettingsComponent.LastExitTimestamp = Time + duration;

                if (init)
                    return;

                trafficPublicExitSettingsComponent.CurrentPedestrianExitCount--;
                carCapacityComponent.AvailableCapacity++;
                carCapacityComponent.AvailableCapacity = math.min(carCapacityComponent.AvailableCapacity, carCapacityComponent.MaxCapacity);

                TrafficDestinationComponent destinationComponent = default;

                if (!TrafficWagonComponentLookup.HasComponent(entity))
                {
                    destinationComponent = TrafficDestinationComponentLookup[entity];
                }
                else
                {
                    var trafficWagonComponent = TrafficWagonComponentLookup[entity];
                    destinationComponent = TrafficDestinationComponentLookup[trafficWagonComponent.OwnerEntity];
                }

                var trafficNodeEntity = destinationComponent.CurrentNode;

                if (ConnectedPedestrianNodeElementLookup.HasBuffer(trafficNodeEntity))
                {
                    bool hasSpawnPosition = false;
                    float3 spawnPosition = default;

                    if (EntityPrefab != Entity.Null && vehicleEntryBuffer.Length > 0)
                    {
                        var rndGen = UnityMathematicsExtension.GetRandomGen(Time, entity.Index, trafficPublicExitSettingsComponent.CurrentPedestrianExitCount);
                        var randomEntry = vehicleEntryBuffer[rndGen.NextInt(0, vehicleEntryBuffer.Length)].EntryPointEntity;
                        spawnPosition = WorldTransformLookup[randomEntry].Position;
                        hasSpawnPosition = true;
                    }

                    var connectedBuffer = ConnectedPedestrianNodeElementLookup[trafficNodeEntity];

                    Entity pedestrianNodeEntity = Entity.Null;

                    if (connectedBuffer.Length == 1)
                    {
                        pedestrianNodeEntity = connectedBuffer[0].PedestrianNodeEntity;
                    }
                    else
                    {
                        float maxDistance = float.MaxValue;

                        for (int i = 0; i < connectedBuffer.Length; i++)
                        {
                            if (connectedBuffer[i].PedestrianNodeEntity == Entity.Null) continue;

                            var currentNodeEntity = connectedBuffer[i].PedestrianNodeEntity;

                            var nodePos = WorldTransformLookup[currentNodeEntity].Position;

                            var distance = math.distancesq(nodePos, spawnPosition);

                            if (distance < maxDistance)
                            {
                                maxDistance = distance;
                                pedestrianNodeEntity = currentNodeEntity;
                            }
                        }
                    }

                    if (pedestrianNodeEntity != Entity.Null && hasSpawnPosition)
                    {
                        var nodeConnectionData = NodeConnectionDataLookup[pedestrianNodeEntity];

                        var soundId = SoundConfigReference.Config.Value.ExitTramSoundId;
                        CommandBuffer.CreateSoundEntity(SoundPrefabEntity, soundId, spawnPosition);

                        ref var pedestrianSettings = ref PedestrianSettingsReference.Config;

                        var seed = UnityMathematicsExtension.GetSeed(Time, entity.Index);

                        var parkingData = new ParkingSpawnHelper.ParkingSpawnData()
                        {
                            Seed = seed,
                            PedestrianEntityPrefab = EntityPrefab,
                            SpawnPointEntity = pedestrianNodeEntity,
                            NodeConnectionData = nodeConnectionData,
                            WorldTransformLookup = WorldTransformLookup,
                            HasCustomSpawnPosition = true,
                            CustomSpawnPosition = spawnPosition
                        };

                        ParkingSpawnHelper.Spawn(ref CommandBuffer, in parkingData, in pedestrianSettings);
                    }
                }

                if (trafficPublicExitSettingsComponent.CurrentPedestrianExitCount <= 0)
                {
                    trafficPublicProccessExitTagRW.ValueRW = false;
                    trafficPublicExitCompleteTagRW.ValueRW = true;
                }
            }
        }
    }
}