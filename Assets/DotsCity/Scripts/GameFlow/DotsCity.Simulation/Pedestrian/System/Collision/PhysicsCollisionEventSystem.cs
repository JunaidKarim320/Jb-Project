﻿using Spirit604.DotsCity.Simulation.Traffic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [UpdateInGroup(typeof(PhysicsTriggerGroup))]
    [BurstCompile]
    unsafe public partial struct PhysicsCollisionEventSystem : ISystem
    {
        private EntityQuery npcQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            npcQuery = SystemAPI.QueryBuilder()
                .WithAll<CollisionComponent, PhysicsVelocity>()
                .Build();

            state.RequireForUpdate(npcQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            state.Dependency = new CollisionEventImpulseJob
            {
                CommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
                CollisionEventGroup = SystemAPI.GetComponentLookup<CarCollisionComponent>(true),
                HasCollisionGroup = SystemAPI.GetComponentLookup<HasCollisionTag>(true),
                LocalTransformGroup = SystemAPI.GetComponentLookup<LocalTransform>(true),
                NpcCollisionEventGroup = SystemAPI.GetComponentLookup<CollisionComponent>(false),
                PhysicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld,
                Timestamp = (float)SystemAPI.Time.ElapsedTime,
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
        }

        [BurstCompile]
        struct CollisionEventImpulseJob : ICollisionEventsJob
        {
            public EntityCommandBuffer CommandBuffer;

            [ReadOnly] public ComponentLookup<CarCollisionComponent> CollisionEventGroup;
            [ReadOnly] public ComponentLookup<HasCollisionTag> HasCollisionGroup;
            [ReadOnly] public ComponentLookup<LocalTransform> LocalTransformGroup;
            public ComponentLookup<CollisionComponent> NpcCollisionEventGroup;

            [ReadOnly] public float Timestamp;
            [ReadOnly] public PhysicsWorld PhysicsWorld;

            public void Execute(CollisionEvent collisionEvent)
            {
                Entity entityA = collisionEvent.EntityA;
                Entity entityB = collisionEvent.EntityB;

                bool isCar = (CollisionEventGroup.HasComponent(entityA) && !CollisionEventGroup.HasComponent(entityB)) || (!CollisionEventGroup.HasComponent(entityA) && CollisionEventGroup.HasComponent(entityB));
                bool isNpc = (NpcCollisionEventGroup.HasComponent(entityA) && !NpcCollisionEventGroup.HasComponent(entityB)) || (!NpcCollisionEventGroup.HasComponent(entityA) && NpcCollisionEventGroup.HasComponent(entityB));

                float3 carPosition = default;

                if (!isCar || !isNpc)
                {
                    return;
                }

                CollisionComponent collisionComponent = default;

                if (NpcCollisionEventGroup.HasComponent(entityA))
                {
                    collisionComponent = NpcCollisionEventGroup[entityA];
                    carPosition = LocalTransformGroup[entityB].Position;

                    if (HasCollisionGroup.HasComponent(entityA))
                    {
                        CommandBuffer.SetComponentEnabled<HasCollisionTag>(entityA, true);
                    }
                }
                else if (NpcCollisionEventGroup.HasComponent(entityB))
                {
                    collisionComponent = NpcCollisionEventGroup[entityB];
                    carPosition = LocalTransformGroup[entityA].Position;

                    if (HasCollisionGroup.HasComponent(entityB))
                    {
                        CommandBuffer.SetComponentEnabled<HasCollisionTag>(entityB, true);
                    }
                }

                var collisionDefails = collisionEvent.CalculateDetails(ref PhysicsWorld);

                collisionComponent.CollidablePosition = carPosition;
                collisionComponent.Position = collisionDefails.AverageContactPointPosition;
                collisionComponent.Force = collisionEvent.Normal * collisionDefails.EstimatedImpulse;
                collisionComponent.LastCollisionTimestamp = Timestamp;

                if (collisionComponent.FirstCollisionTime == 0)
                {
                    collisionComponent.FirstCollisionTime = Timestamp;
                }

                if (NpcCollisionEventGroup.HasComponent(entityA))
                {
                    NpcCollisionEventGroup[entityA] = collisionComponent;
                }
                else if (NpcCollisionEventGroup.HasComponent(entityB))
                {
                    NpcCollisionEventGroup[entityB] = collisionComponent;
                }
            }
        }
    }
}