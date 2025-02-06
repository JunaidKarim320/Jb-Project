using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Npc;
using Spirit604.DotsCity.Simulation.Pedestrian.State;
using Spirit604.DotsCity.Simulation.Sound.Pedestrian;
using Spirit604.DotsCity.Simulation.Sound.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [UpdateInGroup(typeof(PedestrianSimulationGroup))]
    [BurstCompile]
    public partial struct HealthWithRagdollSystem : ISystem
    {
        private EntityQuery updateGroup;
        private EntityQuery soundPrefabQuery;

        void ISystem.OnCreate(ref SystemState state)
        {
            soundPrefabQuery = SoundExtension.GetSoundQuery(state.EntityManager);

            updateGroup = SystemAPI.QueryBuilder()
                .WithDisabled<AliveTag>()
                .WithDisabledRW<RagdollActivateEventTag>()
                .WithAll<StateComponent, RagdollComponent>()
                .Build();

            state.RequireForUpdate(updateGroup);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var activateRagdollJob = new ActivateRagdollJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
                InViewOfCameraLookup = SystemAPI.GetComponentLookup<InViewOfCameraTag>(true),
                SoundConfigReference = SystemAPI.GetSingleton<SoundConfigReference>(),
                SoundPrefabEntity = soundPrefabQuery.GetSingletonEntity(),
            };

            activateRagdollJob.Schedule();
        }

        [WithDisabled(typeof(AliveTag), typeof(PooledEventTag), typeof(RagdollActivateEventTag))]
        [BurstCompile]
        public partial struct ActivateRagdollJob : IJobEntity
        {
            public EntityCommandBuffer CommandBuffer;

            [ReadOnly]
            public ComponentLookup<InViewOfCameraTag> InViewOfCameraLookup;

            [ReadOnly]
            public SoundConfigReference SoundConfigReference;

            [ReadOnly]
            public Entity SoundPrefabEntity;

            void Execute(
                Entity entity,
                ref RagdollComponent ragdollComponent,
                ref HealthComponent healthComponent,
                EnabledRefRW<PooledEventTag> pooledEventTagRW,
                EnabledRefRW<RagdollActivateEventTag> ragdollActivateEventTagRW,
                in LocalTransform transform)
            {
                ragdollComponent.Position = transform.Position;
                ragdollComponent.Rotation = transform.Rotation;
                ragdollComponent.ForceDirection = healthComponent.HitDirection;
                ragdollComponent.ForceMultiplier = healthComponent.ForceMultiplier;

                if (math.isnan(ragdollComponent.Position.x))
                    return;

                bool pooled = InViewOfCameraLookup.HasComponent(entity) && !InViewOfCameraLookup.IsComponentEnabled(entity);

                if (pooled)
                {
                    PoolEntityUtils.DestroyEntity(ref pooledEventTagRW);
                    return;
                }

                ragdollActivateEventTagRW.ValueRW = true;

                var soundId = SoundConfigReference.Config.Value.DeathSoundId;
                CommandBuffer.CreateSoundEntity(SoundPrefabEntity, soundId, transform.Position);
            }
        }
    }
}