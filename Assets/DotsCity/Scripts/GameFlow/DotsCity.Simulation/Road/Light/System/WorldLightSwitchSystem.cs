using Spirit604.DotsCity.Simulation.Level.Props;
using Spirit604.Gameplay.Road;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;

namespace Spirit604.DotsCity.Simulation.Road
{
    [UpdateAfter(typeof(LightHandlerSwitchSystem))]
    [UpdateInGroup(typeof(SimulationGroup))]
    [BurstCompile]
    public partial struct WorldLightSwitchSystem : ISystem
    {
        private EntityQuery updateQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            updateQuery = SystemAPI.QueryBuilder()
                .WithNone<PropsDamagedTag>()
                .WithAll<LightFrameHandlerEntityComponent>()
                .Build();

            state.RequireForUpdate(updateQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var switchLightJob = new SwitchLightJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                LightHandlerLookup = SystemAPI.GetComponentLookup<LightHandlerComponent>(true)
            };

            switchLightJob.ScheduleParallel();
        }

        [WithNone(typeof(PropsDamagedTag))]
        [BurstCompile]
        private partial struct SwitchLightJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter CommandBuffer;

            [ReadOnly]
            public ComponentLookup<LightHandlerComponent> LightHandlerLookup;

            void Execute(
                [ChunkIndexInQuery] int entityInQueryIndex,
                ref LightFrameHandlerStateComponent lightFrameHandlerStateComponent,
                in LightFrameHandlerEntityComponent lightHandlerComponent,
                in LightFrameData lightFrameData)
            {
                if (!LightHandlerLookup.HasComponent(lightHandlerComponent.RelatedEntityHandler))
                    return;

                var lightHandlerData = LightHandlerLookup[lightHandlerComponent.RelatedEntityHandler];
                var newState = lightHandlerData.State;

                if (lightFrameHandlerStateComponent.CurrentLightState != newState)
                {
                    var previousLightState = lightFrameHandlerStateComponent.CurrentLightState;
                    lightFrameHandlerStateComponent.CurrentLightState = newState;

                    SetLightEntityState(ref CommandBuffer, entityInQueryIndex, newState, previousLightState, in lightFrameData);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetLightEntityState(ref EntityCommandBuffer.ParallelWriter commandBuffer, int entityInQueryIndex, LightState newLightState, LightState previousLightState, in LightFrameData light)
        {
            switch (previousLightState)
            {
                case LightState.Uninitialized:
                    break;
                case LightState.RedYellow:
                    {
                        if (light.YellowEntity != Entity.Null)
                        {
                            commandBuffer.SetComponentEnabled<MaterialMeshInfo>(entityInQueryIndex, light.YellowEntity, false);
                        }

                        if (light.RedEntity != Entity.Null)
                        {
                            commandBuffer.SetComponentEnabled<MaterialMeshInfo>(entityInQueryIndex, light.RedEntity, false);
                        }

                        break;
                    }
                case LightState.Green:
                    {
                        commandBuffer.SetComponentEnabled<MaterialMeshInfo>(entityInQueryIndex, light.GreenEntity, false);

                        break;
                    }
                case LightState.Yellow:
                    {
                        if (light.YellowEntity != Entity.Null)
                        {
                            commandBuffer.SetComponentEnabled<MaterialMeshInfo>(entityInQueryIndex, light.YellowEntity, false);
                        }

                        break;
                    }
                case LightState.Red:
                    {
                        if (light.RedEntity != Entity.Null)
                        {
                            commandBuffer.SetComponentEnabled<MaterialMeshInfo>(entityInQueryIndex, light.RedEntity, false);
                        }

                        break;
                    }
            }

            if (light.YellowEntity != Entity.Null)
            {
                switch (newLightState)
                {
                    case LightState.Uninitialized:
                        break;
                    case LightState.RedYellow:
                        {
                            commandBuffer.SetComponentEnabled<MaterialMeshInfo>(entityInQueryIndex, light.YellowEntity, true);
                            commandBuffer.SetComponentEnabled<MaterialMeshInfo>(entityInQueryIndex, light.RedEntity, true);
                            break;
                        }
                    case LightState.Green:
                        {
                            commandBuffer.SetComponentEnabled<MaterialMeshInfo>(entityInQueryIndex, light.GreenEntity, true);
                            break;
                        }
                    case LightState.Yellow:
                        {
                            commandBuffer.SetComponentEnabled<MaterialMeshInfo>(entityInQueryIndex, light.YellowEntity, true);
                            break;
                        }
                    case LightState.Red:
                        {
                            commandBuffer.SetComponentEnabled<MaterialMeshInfo>(entityInQueryIndex, light.RedEntity, true);
                            break;
                        }
                }
            }
            else
            {
                if (newLightState == LightState.Green)
                {
                    commandBuffer.SetComponentEnabled<MaterialMeshInfo>(entityInQueryIndex, light.GreenEntity, true);
                }
                else
                {
                    commandBuffer.SetComponentEnabled<MaterialMeshInfo>(entityInQueryIndex, light.RedEntity, true);
                }
            }
        }
    }
}