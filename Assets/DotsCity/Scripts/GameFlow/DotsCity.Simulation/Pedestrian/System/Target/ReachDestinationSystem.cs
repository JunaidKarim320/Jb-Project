using Spirit604.DotsCity.Simulation.Npc.Navigation;
using Spirit604.DotsCity.Simulation.Pedestrian.State;
using Spirit604.DotsCity.Simulation.Road;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [UpdateAfter(typeof(PedestrianMovementSystem))]
    [UpdateInGroup(typeof(PedestrianFixedSimulationGroup))]
    [BurstCompile]
    public partial struct ReachDestinationSystem : ISystem
    {
        private EntityQuery updateGroup;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            updateGroup = SystemAPI.QueryBuilder()
                .WithDisabled<IdleTag>()
                .WithAll<HasTargetTag, DestinationComponent>()
                .Build();

            state.RequireForUpdate(updateGroup);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var reachDestinationJob = new ReachDestinationJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                EnabledNavigationLookup = SystemAPI.GetComponentLookup<EnabledNavigationTag>(true),
                NodeConnectionBufferLookup = SystemAPI.GetBufferLookup<NodeConnectionDataElement>(true),
                NodeSettingsLookup = SystemAPI.GetComponentLookup<NodeSettingsComponent>(true),
                NodeCapacityLookup = SystemAPI.GetComponentLookup<NodeCapacityComponent>(true),
                NodeLightSettingsComponentLookup = SystemAPI.GetComponentLookup<NodeLightSettingsComponent>(true),
                LightHandlerLookup = SystemAPI.GetComponentLookup<LightHandlerComponent>(true),
                WorldTransformLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true),
                DestinationConfigReference = SystemAPI.GetSingleton<DestinationConfigReference>(),
                Timestamp = (float)SystemAPI.Time.ElapsedTime,
            };

            reachDestinationJob.ScheduleParallel();
        }

        [WithDisabled(typeof(IdleTag))]
        [WithAll(typeof(HasTargetTag))]
        [BurstCompile]
        private partial struct ReachDestinationJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter CommandBuffer;

            [ReadOnly]
            public ComponentLookup<EnabledNavigationTag> EnabledNavigationLookup;

            [ReadOnly]
            public BufferLookup<NodeConnectionDataElement> NodeConnectionBufferLookup;

            [ReadOnly]
            public ComponentLookup<NodeSettingsComponent> NodeSettingsLookup;

            [ReadOnly]
            public ComponentLookup<NodeCapacityComponent> NodeCapacityLookup;

            [ReadOnly]
            public ComponentLookup<NodeLightSettingsComponent> NodeLightSettingsComponentLookup;

            [ReadOnly]
            public ComponentLookup<LightHandlerComponent> LightHandlerLookup;

            [ReadOnly]
            public ComponentLookup<LocalToWorld> WorldTransformLookup;

            [ReadOnly]
            public DestinationConfigReference DestinationConfigReference;

            [ReadOnly]
            public float Timestamp;

            void Execute(
                Entity entity,
                [ChunkIndexInQuery] int entityInQueryIndex,
                ref DestinationComponent destinationComponent,
                ref NextStateComponent nextStateComponent,
                EnabledRefRW<HasTargetTag> hasTargetTagRW,
                in LocalToWorld worldTransform)
            {
                float distanceSQ = math.distancesq(worldTransform.Position, destinationComponent.Value);
                destinationComponent.DestinationDistanceSQ = distanceSQ;

                float achieveDistanceSQ = destinationComponent.CustomAchieveDistanceSQ == 0 ?
                      DestinationConfigReference.Config.Value.AchieveDistanceSQ : destinationComponent.CustomAchieveDistanceSQ;

                if (distanceSQ < achieveDistanceSQ)
                {
                    if (EnabledNavigationLookup.HasComponent(entity) && EnabledNavigationLookup.IsComponentEnabled(entity))
                    {
                        CommandBuffer.SetComponentEnabled<EnabledNavigationTag>(entityInQueryIndex, entity, false);
                    }

                    SelectAchievedTargetUtils.ProcessAchievedTarget(
                        ref CommandBuffer,
                        in NodeConnectionBufferLookup,
                        in NodeSettingsLookup,
                        in NodeCapacityLookup,
                        in NodeLightSettingsComponentLookup,
                        in LightHandlerLookup,
                        in WorldTransformLookup,
                        in DestinationConfigReference,
                        in Timestamp,
                        entity,
                        entityInQueryIndex,
                        ref destinationComponent,
                        ref nextStateComponent,
                        ref hasTargetTagRW,
                        in worldTransform);
                }
            }
        }
    }
}