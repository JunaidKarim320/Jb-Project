using Spirit604.AnimationBaker.Entities;
using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Hybrid.Core;
using Spirit604.DotsCity.Simulation.Common;
using Spirit604.DotsCity.Simulation.Config;
using Spirit604.DotsCity.Simulation.Npc;
using Spirit604.DotsCity.Simulation.Npc.Navigation;
using Spirit604.DotsCity.Simulation.Pedestrian.State;
using Unity.Entities;
using Unity.Entities.Graphics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Material = UnityEngine.Material;

namespace Spirit604.DotsCity.Simulation.Pedestrian.Authoring
{
    public static class PedestrianEntityBakingUtils
    {
        public static void Bake(
            ref EntityCommandBuffer commandBuffer,
            ref EntityManager entityManager,
            Entity entity,
            in MiscConversionSettingsReference conversionSettings,
            in GeneralCoreSettingsDataReference coreSettingsData,
            in CommonGeneralSettingsReference commonGeneralSettingsData,
            in PedestrianGeneralSettingsReference pedestrianGeneralSettingsData,
            in PedestrianSettingsReference pedestrianSettingsReference)
        {
            if (entityManager.HasComponent<CircleColliderComponent>(entity))
            {
                CircleColliderComponent circleColliderComponent = new CircleColliderComponent()
                {
                    Radius = conversionSettings.Config.Value.PedestrianColliderRadius
                };

                commandBuffer.SetComponent(entity, circleColliderComponent);
            }

            var dotsSimulation = coreSettingsData.Config.Value.DOTSSimulation;
            bool hasPhysics = coreSettingsData.Config.Value.SimulationType != SimulationType.NoPhysics &&
                (conversionSettings.Config.Value.EntityType == EntityType.Physics || conversionSettings.Config.Value.CollisionType == CollisionType.Physics);

            if (hasPhysics)
            {
                if (coreSettingsData.Config.Value.CullPhysics)
                {
                    if (entityManager.HasComponent<PhysicsCollider>(entity))
                    {
                        commandBuffer.AddComponent<CullPhysicsTag>(entity);

                        commandBuffer.SetSharedComponent(entity, new PhysicsWorldIndex()
                        {
                            Value = ProjectConstants.NoPhysicsWorldIndex
                        });
                    }
                }
            }
            else
            {
                if (entityManager.HasComponent<PhysicsCollider>(entity))
                {
                    commandBuffer.RemoveComponent<PhysicsCollider>(entity);
                    commandBuffer.RemoveComponent<PhysicsVelocity>(entity);
                    commandBuffer.RemoveComponent<PhysicsMass>(entity);
                }
            }

            var navAgentComponent = new NavAgentComponent();

            var avoidanceType = pedestrianGeneralSettingsData.Config.Value.NavigationSupport ? conversionSettings.Config.Value.ObstacleAvoidanceType : ObstacleAvoidanceType.Disabled;

            if (avoidanceType != ObstacleAvoidanceType.Disabled)
            {
                commandBuffer.AddComponent<EnabledNavigationTag>(entity);
                commandBuffer.SetComponentEnabled<EnabledNavigationTag>(entity, false);

                switch (avoidanceType)
                {
                    case ObstacleAvoidanceType.CalcNavPath:
                        {
                            commandBuffer.AddComponent<NavAgentTag>(entity);

#if REESE_PATH
                            Reese.Path.PathAgentAuthoring.AddComponents(ref commandBuffer, entity, "Humanoid", Vector3.zero);
#else
                            Debug.Log("PedestrianEntityBakingUtils. CalcNavPath avoidance type is selected but the Reese navigation package is not installed, install this package, otherwise select a different avoidance method in the Pedestrian settings.");
#endif

                            if (conversionSettings.Config.Value.PedestrianNavigationType == NpcNavigationType.Persist)
                            {
                                commandBuffer.AddComponent<PersistNavigationTag>(entity);
                                commandBuffer.AddComponent<PersistNavigationComponent>(entity);
                            }
                            else
                            {
                                commandBuffer.SetComponentEnabled<UpdateNavTargetTag>(entity, false);
                            }

                            break;
                        }
                    case ObstacleAvoidanceType.LocalAvoidance:
                        {
                            commandBuffer.SetComponentEnabled<UpdateNavTargetTag>(entity, false);
                            commandBuffer.AddComponent<LocalAvoidanceAgentTag>(entity);
                            commandBuffer.AddBuffer<PathPointAvoidanceElement>(entity);
                            commandBuffer.AddComponent<PathLocalAvoidanceEnabledTag>(entity);
                            commandBuffer.SetComponentEnabled<PathLocalAvoidanceEnabledTag>(entity, false);

                            break;
                        }
                    case ObstacleAvoidanceType.AgentsNavigation:
                        {
                            commandBuffer.AddComponent<CustomLocomotionTag>(entity);
                            commandBuffer.AddComponent<PersistNavigationTag>(entity);
                            commandBuffer.AddComponent<PersistNavigationComponent>(entity);
                            commandBuffer.AddComponent<AgentInitTag>(entity);
                            break;
                        }
                }
            }

            commandBuffer.SetComponent(entity, navAgentComponent);

            if (!commonGeneralSettingsData.Config.Value.HealthSupport)
            {
                if (entityManager.HasComponent<HealthComponent>(entity))
                {
                    commandBuffer.RemoveComponent<HealthComponent>(entity);
                }
            }
            else
            {
                var hasRagdoll = conversionSettings.Config.Value.HasRagdoll &&
                    (coreSettingsData.Config.Value.SimulationType != SimulationType.NoPhysics || !dotsSimulation);

                if (hasRagdoll)
                {
                    commandBuffer.AddComponent<RagdollComponent>(entity);
                    commandBuffer.AddComponent<RagdollActivateEventTag>(entity);
                    commandBuffer.SetComponentEnabled<RagdollActivateEventTag>(entity, false);

                    if (conversionSettings.Config.Value.RagdollType == RagdollType.Custom)
                    {
                        commandBuffer.AddComponent<CustomRagdollTag>(entity);
                    }
                }
            }

            bool hasFaction = commonGeneralSettingsData.Config.Value.BulletSupport;

            if (hasFaction)
            {
                commandBuffer.AddComponent(entity, new FactionTypeComponent { Value = FactionType.City });
            }

            if (conversionSettings.Config.Value.PedestrianSkinType == NpcSkinType.RigShowAlways)
            {
                commandBuffer.AddComponent<DisableUnloadSkinTag>(entity);
            }

            if (conversionSettings.Config.Value.PedestrianSkinType == NpcSkinType.DummyShowAlways)
            {
                commandBuffer.AddComponent<DisableUnloadSkinTag>(entity);
            }

            bool hasEnityRenderSkin = false;

            var npcRigType = conversionSettings.Config.Value.PedestrianRigType;

            if (conversionSettings.Config.Value.HasRig)
            {
                switch (npcRigType)
                {
                    case NpcRigType.HybridLegacy:
                        {
                            AddHybridSkinComponents(ref commandBuffer, entity);
                            break;
                        }
                    case NpcRigType.PureGPU:
                        {
                            AddGPUSkinComponents(ref commandBuffer, entity);
                            hasEnityRenderSkin = true;
                            break;
                        }
                    case NpcRigType.HybridAndGPU:
                        {
                            AddHybridSkinComponents(ref commandBuffer, entity);
                            AddGPUSkinComponents(ref commandBuffer, entity);

                            commandBuffer.SetComponentEnabled<GPUSkinTag>(entity, false);
                            commandBuffer.SetComponentEnabled<HybridLegacySkinTag>(entity, false);
                            commandBuffer.AddComponent<HybridGPUSkinTag>(entity);
                            commandBuffer.AddComponent<HybridGPUSkinData>(entity);
                            commandBuffer.AddComponent<DisableUnloadSkinTag>(entity);
                            hasEnityRenderSkin = true;
                            break;
                        }
                }
            }

            if (hasEnityRenderSkin)
            {
                AddRenderComponents(ref commandBuffer, entity);
            }

            AddAnimatorCustomStateComponents(ref commandBuffer, entity);

            var addDummy = (conversionSettings.Config.Value.PedestrianSkinType == NpcSkinType.RigAndDummy) ||
                    (conversionSettings.Config.Value.PedestrianSkinType == NpcSkinType.DummyShowAlways) ||
                    (conversionSettings.Config.Value.PedestrianSkinType == NpcSkinType.DummyShowOnlyInView);

            if (addDummy)
            {
                if (conversionSettings.Config.Value.PedestrianSkinType == NpcSkinType.DummyShowOnlyInView)
                {
                    commandBuffer.AddComponent<LoadDummySkinInViewTag>(entity);
                }

                if (conversionSettings.Config.Value.PedestrianSkinType == NpcSkinType.RigAndDummy)
                {
                    commandBuffer.AddComponent<LoadDummySkinIfOutOfCameraTag>(entity);
                }

                AddDummySkinComponents(ref commandBuffer, ref entityManager, entity, conversionSettings, npcRigType);
            }
        }

        private static void AddHybridSkinComponents(ref EntityCommandBuffer commandBuffer, Entity entity)
        {
            commandBuffer.AddComponent<HybridLegacySkinTag>(entity);
            commandBuffer.AddComponent<CopyTransformToGameObject>(entity);
            commandBuffer.SetComponentEnabled<CopyTransformToGameObject>(entity, false);
        }

        private static void AddGPUSkinComponents(ref EntityCommandBuffer commandBuffer, Entity entity)
        {
            commandBuffer.AddComponent<GPUSkinTag>(entity);
            commandBuffer.AddComponent<SkinUpdateComponent>(entity);
            commandBuffer.AddComponent<UpdateSkinTag>(entity);
            commandBuffer.AddComponent<SkinAnimatorData>(entity);

            commandBuffer.AddComponent(entity, new AnimationTransitionData()
            {
                LastAnimHash = -1
            });

            commandBuffer.AddComponent<HasAnimTransitionTag>(entity);

            ShaderUtils.AddShaderComponents(ref commandBuffer, entity, true);

            commandBuffer.SetComponentEnabled<UpdateSkinTag>(entity, false);
            commandBuffer.SetComponentEnabled<HasAnimTransitionTag>(entity, false);

            commandBuffer.AddComponent<RenderBounds>(entity);
            commandBuffer.AddComponent<WorldRenderBounds>(entity);
        }

        private static void AddRenderComponents(ref EntityCommandBuffer commandBuffer, Entity entity)
        {
            commandBuffer.AddComponent<WorldToLocal_Tag>(entity);
            commandBuffer.AddComponent<PerInstanceCullingTag>(entity);
            commandBuffer.AddComponent<BlendProbeTag>(entity);
            commandBuffer.AddComponent<MaterialMeshInfo>(entity);
            commandBuffer.SetComponentEnabled<MaterialMeshInfo>(entity, false);

            var filterSettings = RenderFilterSettings.Default;
            filterSettings.ShadowCastingMode = ShadowCastingMode.On;
            filterSettings.ReceiveShadows = false;

            var renderMeshDescription = new RenderMeshDescription
            {
                FilterSettings = filterSettings,
                LightProbeUsage = LightProbeUsage.Off,
            };

            commandBuffer.AddSharedComponentManaged(entity, filterSettings);
        }

        private static void AddAnimatorCustomStateComponents(ref EntityCommandBuffer commandBuffer, Entity entity)
        {
            commandBuffer.AddComponent<CustomAnimatorStateTag>(entity);
            commandBuffer.AddComponent<HasCustomAnimationTag>(entity);
            commandBuffer.AddComponent<UpdateCustomAnimationTag>(entity);
            commandBuffer.AddComponent<WaitForCustomAnimationTag>(entity);
            commandBuffer.AddComponent<ExitCustomAnimationTag>(entity);

            commandBuffer.SetComponentEnabled<CustomAnimatorStateTag>(entity, false);
            commandBuffer.SetComponentEnabled<HasCustomAnimationTag>(entity, false);
            commandBuffer.SetComponentEnabled<UpdateCustomAnimationTag>(entity, false);
            commandBuffer.SetComponentEnabled<WaitForCustomAnimationTag>(entity, false);
            commandBuffer.SetComponentEnabled<ExitCustomAnimationTag>(entity, false);
        }

        private static void AddDummySkinComponents(ref EntityCommandBuffer commandBuffer, ref EntityManager entityManager, Entity entity, in MiscConversionSettingsReference conversionSettings, NpcRigType npcRigType)
        {
            var dummyPrefabQuery = entityManager.CreateEntityQuery(typeof(DummyPrefabData));
            var dummyPrefabEntity = dummyPrefabQuery.GetSingletonEntity();
            var dummyRenderMesh = entityManager.GetSharedComponentManaged<MyRenderMesh>(dummyPrefabEntity);

            if (dummyRenderMesh.mesh == null)
                return;

            var dummyPrefabData = entityManager.GetComponentData<DummyPrefabData>(dummyPrefabEntity);

            commandBuffer.AddComponent<DummySkinEnabledTag>(entity);
            commandBuffer.SetComponentEnabled<DummySkinEnabledTag>(entity, false);

            var dummyChildEntity = entityManager.CreateEntity(typeof(LocalTransform), typeof(LocalToWorld), typeof(Parent), typeof(PreviousParent));

            AddRenderComponents(ref commandBuffer, dummyChildEntity);

            var unloadRenderBounds =
                (conversionSettings.Config.Value.PedestrianSkinType == NpcSkinType.RigAndDummy) ||
                conversionSettings.Config.Value.PedestrianSkinType == NpcSkinType.DummyShowOnlyInView;

            commandBuffer.AddComponent(entity, new DummySkinData()
            {
                UnloadRenderBounds = unloadRenderBounds,
                DummyEntity = dummyChildEntity,
            });

            commandBuffer.AddComponent(dummyChildEntity, new RenderBounds()
            {
                Value = new Unity.Mathematics.AABB()
                {
                    Center = dummyRenderMesh.mesh.bounds.center,
                    Extents = dummyRenderMesh.mesh.bounds.extents,
                }
            });

            commandBuffer.AddComponent<WorldRenderBounds>(dummyChildEntity);

            RenderMeshArray renderMeshArray = new RenderMeshArray(new Material[]
            {
                       dummyRenderMesh.material,
            },
            new Mesh[]
            {
                       dummyRenderMesh.mesh
            });

            DynamicBuffer<LinkedEntityGroup> linkedEntities = default;

            if (entityManager.HasBuffer<LinkedEntityGroup>(entity))
            {
                linkedEntities = entityManager.GetBuffer<LinkedEntityGroup>(entity);
            }
            else
            {
                linkedEntities = entityManager.AddBuffer<LinkedEntityGroup>(entity);
            }

            linkedEntities.Add(new LinkedEntityGroup()
            {
                Value = entity
            });

            linkedEntities.Add(new LinkedEntityGroup()
            {
                Value = dummyChildEntity
            });

            DynamicBuffer<Child> childBuffer = default;

            if (entityManager.HasBuffer<Child>(entity))
            {
                childBuffer = entityManager.GetBuffer<Child>(entity);
            }
            else
            {
                childBuffer = entityManager.AddBuffer<Child>(entity);
            }

            childBuffer.Add(new Child()
            {
                Value = dummyChildEntity
            });

            commandBuffer.SetComponent(dummyChildEntity, new Parent()
            {
                Value = entity
            });

            commandBuffer.SetComponent(dummyChildEntity, new PreviousParent()
            {
                Value = entity
            });

            commandBuffer.SetComponent(dummyChildEntity, LocalTransform.FromScale(dummyPrefabData.Scale));

            commandBuffer.AddSharedComponentManaged(dummyChildEntity, renderMeshArray);
        }
    }
}