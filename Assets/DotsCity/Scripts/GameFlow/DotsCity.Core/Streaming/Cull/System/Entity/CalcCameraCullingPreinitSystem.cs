using Spirit604.Extensions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Spirit604.DotsCity.Core
{
    [UpdateAfter(typeof(InitCameraCullingSystem))]
    [UpdateInGroup(typeof(CullSimulationGroup))]
    [BurstCompile]
    public partial struct CalcCameraCullingPreinitSystem : ISystem
    {
        private EntityQuery cullPointGroup;
        private EntityQuery cameraDataGroup;
        private EntityQuery cullGroup;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            cullPointGroup = SystemAPI.QueryBuilder()
                .WithAll<CullPointTag, LocalTransform>()
                .Build();

            cameraDataGroup = SystemAPI.QueryBuilder()
                .WithAll<CameraData>()
                .Build();

            cullGroup = SystemAPI.QueryBuilder()
                .WithNone<PooledEventTag, CullSharedConfig, CullCameraSharedConfig>()
                .WithAllRW<CullStateComponent>()
                .WithPresentRW<CulledEventTag, InPermittedRangeTag>()
                .WithPresentRW<InViewOfCameraTag, PreInitInCameraTag>()
                .WithAll<LocalTransform>()
                .Build();

            state.RequireForUpdate(cullPointGroup);
            state.RequireForUpdate(cullGroup);
            state.RequireForUpdate(cameraDataGroup);
            state.RequireForUpdate<CullSystemConfigReference>();
            state.Enabled = false;
        }

        [BurstCompile(DisableSafetyChecks = true)]
        void ISystem.OnUpdate(ref SystemState state)
        {
            state.Dependency.Complete();

            var calcCullJob = new CalcCullJob()
            {
                CullPointPosition = cullPointGroup.GetSingleton<LocalTransform>().Position,
                ViewProjectionMatrix = cameraDataGroup.GetSingleton<CameraData>().ViewProjectionMatrix,
                Config = SystemAPI.GetSingleton<CullSystemConfigReference>()
            };

            calcCullJob.ScheduleParallel(cullGroup);
        }

        [BurstCompile(DisableSafetyChecks = true)]
        public partial struct CalcCullJob : IJobEntity
        {
            [ReadOnly]
            public float3 CullPointPosition;

            [ReadOnly]
            public Matrix4x4 ViewProjectionMatrix;

            [ReadOnly]
            public CullSystemConfigReference Config;

            void Execute(
                ref CullStateComponent cullComponent,
                EnabledRefRW<CulledEventTag> culledTagRW,
                EnabledRefRW<InPermittedRangeTag> inPermittedRangeTagRW,
                EnabledRefRW<PreInitInCameraTag> preinitTagRW,
                EnabledRefRW<InViewOfCameraTag> inViewOfCameraTagRW,
                in LocalTransform transform)
            {
                float distance = 0;

                if (!Config.Config.Value.IgnoreY)
                {
                    distance = math.distancesq(transform.Position, CullPointPosition);
                }
                else
                {
                    distance = math.distancesq(transform.Position.Flat(), CullPointPosition.Flat());
                }

                bool inViewOfCamera = CullUtils.InViewOfCamera(ViewProjectionMatrix, transform.Position, Config.Config.Value.ViewPortOffset);

                CullState cullState = CullState.Culled;

                if (inViewOfCamera)
                {
                    if (distance < Config.Config.Value.VisibleDistanceSQ)
                    {
                        cullState = CullState.InViewOfCamera;
                    }
                    else if (distance < Config.Config.Value.PreinitDistanceSQ)
                    {
                        cullState = CullState.PreInitInCamera;
                    }
                    else if (distance < Config.Config.Value.MaxDistanceSQ)
                    {
                        cullState = CullState.CloseToCamera;
                    }
                }
                else
                {
                    if (distance < Config.Config.Value.BehindVisibleDistanceSQ)
                    {
                        cullState = CullState.InViewOfCamera;
                    }
                    else if (distance < Config.Config.Value.BehindPreinitDistanceSQ)
                    {
                        cullState = CullState.PreInitInCamera;
                    }
                    else if (distance < Config.Config.Value.BehindMaxDistanceSQ)
                    {
                        cullState = CullState.CloseToCamera;
                    }
                }

                if (cullComponent.State != cullState)
                {
                    CullStatePreinitUtils.ChangeState(
                        in cullState,
                        ref cullComponent,
                        ref culledTagRW,
                        ref inPermittedRangeTagRW,
                        ref preinitTagRW,
                        ref inViewOfCameraTagRW);
                }
            }
        }
    }
}