using Spirit604.AnimationBaker.Entities;
using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Hybrid.Core;
using Spirit604.DotsCity.Simulation.Pedestrian.State;
using Spirit604.Extensions;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [UpdateInGroup(typeof(LateInitGroup))]
    [RequireMatchingQueriesForUpdate]
    public partial class UnloadHybridGPUSkinSystem : EndInitSystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Enabled = false;
            RequireForUpdate<AnimatorDataProviderSystem.Singleton>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = GetCommandBuffer();

            float timestamp = (float)SystemAPI.Time.ElapsedTime;

            var animatorDataProvider = SystemAPI.GetSingleton<AnimatorDataProviderSystem.Singleton>();
            var crowdSkinProvider = SystemAPI.GetSingleton<CrowdSkinProviderSystem.Singleton>();
            var updateCustomAnimationLookup = SystemAPI.GetComponentLookup<CustomAnimatorStateTag>(true);
            var takenAnimationDataLookup = SystemAPI.GetComponentLookup<TakenAnimationDataComponent>(true);

            Entities
            .WithoutBurst()
            .WithReadOnly(updateCustomAnimationLookup)
            .WithReadOnly(takenAnimationDataLookup)
            .WithNone<ForceHybridGPUSkinTag>()
            .WithDisabled<InViewOfCameraTag, GPUSkinTag>()
            .WithAny<InPermittedRangeTag, PreInitInCameraTag>()
            .WithAll<HybridLegacySkinTag>()
            .ForEach((
                Entity entity,
                Transform transform,
                Animator animator,
                ref SkinUpdateComponent skinUpdateComponent,
                ref PedestrianCommonSettings pedestrianCommonSettings,
                ref SkinAnimatorData skinAnimatorData,
                ref AnimationStateComponent animationStateComponent,
                in StateComponent stateComponent) =>
            {
                if (timestamp - pedestrianCommonSettings.LoadSkinTimestamp < 0.4f)
                    return;

                pedestrianCommonSettings.LoadSkinTimestamp = timestamp;

                var state = animator.GetCurrentAnimatorStateInfo(0);
                var startTime = state.normalizedTime * state.length;

                if (animatorDataProvider.PlayGPUAnimation(ref skinUpdateComponent, animationStateComponent.AnimationState, startTime))
                {
                    var animationHash = animatorDataProvider.GetGPUAnimationData(animationStateComponent.AnimationState).AnimationHash;
                    transform.gameObject.GetComponent<PoolableObjectInfo>().ReturnToPool();

                    commandBuffer.RemoveComponent<Transform>(entity);
                    commandBuffer.RemoveComponent<Animator>(entity);

                    commandBuffer.SetComponentEnabled<CopyTransformToGameObject>(entity, false);
                    commandBuffer.SetComponentEnabled<HybridLegacySkinTag>(entity, false);
                    commandBuffer.SetComponentEnabled<GPUSkinTag>(entity, true);
                    commandBuffer.SetComponentEnabled<MaterialMeshInfo>(entity, true);

                    bool updateSkin = true;
                    bool customAnimatorState = updateCustomAnimationLookup.HasComponent(entity) && updateCustomAnimationLookup.IsComponentEnabled(entity);

                    if (takenAnimationDataLookup.HasComponent(entity))
                    {
                        if (skinAnimatorData.CurrentAnimationHash != animationHash)
                        {
                            var takenMeshIndex = takenAnimationDataLookup[entity].TakenMeshIndex;
                            crowdSkinProvider.TryToRemoveIndex(takenMeshIndex);
                            commandBuffer.RemoveComponent<TakenAnimationDataComponent>(entity);
                        }

                        if (customAnimatorState)
                        {
                            commandBuffer.SetComponentEnabled<UpdateCustomAnimationTag>(entity, true);
                            animationStateComponent.NewStartPlaybacktime = startTime;
                            animationStateComponent.NewAnimationState = animationStateComponent.AnimationState;
                            updateSkin = false;
                        }
                    }
                    else
                    {
                        if (customAnimatorState)
                        {
                            commandBuffer.SetComponentEnabled<UpdateCustomAnimationTag>(entity, true);
                            animationStateComponent.NewStartPlaybacktime = startTime;
                            animationStateComponent.NewAnimationState = animationStateComponent.AnimationState;
                            updateSkin = false;
                        }
                    }

                    if (updateSkin)
                        commandBuffer.SetComponentEnabled<UpdateSkinTag>(entity, true);

                    commandBuffer.SetSharedComponent(entity, new WorldEntitySharedType(EntityWorldType.PureEntity));
                }

            }).Run();


            AddCommandBufferForProducer();
        }
    }
}