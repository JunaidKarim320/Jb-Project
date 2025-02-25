using Spirit604.AnimationBaker;
using Spirit604.AnimationBaker.Entities;
using Spirit604.AnimationBaker.Entities.Jobs;
using Spirit604.DotsCity.Core;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine.Rendering;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    [UpdateInGroup(typeof(LateSimulationGroup))]
    [BurstCompile]
    public partial struct UpdateGPUSkinSystem : ISystem, ISystemStartStop
    {
        private EntityQuery npcQuery;
        private NativeHashMap<SkinAnimationHash, HashToIndexData> hashToLocalDataLocalRef;
        private NativeHashSet<int> takenIndexesLocalRef;
        private NativeHashMap<int, BatchMeshID> meshMappingLocalRef;
        private NativeHashMap<int, BatchMaterialID> materialMappingLocalRef;
        private NativeHashSet<int> allowDuplicateHashesLocalRef;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            npcQuery = SystemAPI.QueryBuilder()
                .WithNone<InPermittedRangeTag, DisableUnloadSkinTag>()
                .WithAll<HasSkinTag, GPUSkinTag, UpdateSkinTag>()
                .WithAllRW<SkinUpdateComponent, SkinAnimatorData>()
                .WithAllRW<ShaderTargetFrameOffsetData, ShaderTargetFrameStepInvData>()
                .WithAllRW<ShaderPlaybackTime, MaterialMeshInfo>()
                .Build();

            state.RequireForUpdate(npcQuery);
        }

        void ISystem.OnDestroy(ref SystemState state)
        {
            hashToLocalDataLocalRef = default;
            takenIndexesLocalRef = default;
            meshMappingLocalRef = default;
            materialMappingLocalRef = default;
            allowDuplicateHashesLocalRef = default;
        }

        void ISystemStartStop.OnStartRunning(ref SystemState state)
        {
            hashToLocalDataLocalRef = CrowdSkinProviderSystem.HashToLocalDataStaticRef;
            takenIndexesLocalRef = CrowdSkinProviderSystem.TakenIndexesStaticRef;
            meshMappingLocalRef = CrowdSkinProviderSystem.MeshMappingStaticRef;
            materialMappingLocalRef = CrowdSkinProviderSystem.MaterialMappingStaticRef;
            allowDuplicateHashesLocalRef = CrowdSkinProviderSystem.AllowDuplicateHashesStaticRef;
        }

        void ISystemStartStop.OnStopRunning(ref SystemState state) { }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var updateSkinJob = new UpdateGPUSkinJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
                AnimationBlobReference = SystemAPI.GetSingleton<AnimationBlobReference>(),
                TakenAnimationDataLookup = SystemAPI.GetComponentLookup<TakenAnimationDataComponent>(false),
                TakenIndexes = takenIndexesLocalRef,
                HashToLocalData = hashToLocalDataLocalRef,
                AllowDuplicateHashes = allowDuplicateHashesLocalRef,
                MaterialMapping = materialMappingLocalRef,
                MeshMapping = meshMappingLocalRef,
                CurrentTime = (float)SystemAPI.Time.ElapsedTime
            };

            updateSkinJob.Schedule(npcQuery);
        }
    }
}