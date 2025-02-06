#if FMOD
using FMOD.Studio;
using FMODUnity;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Sound
{
    [UpdateInGroup(typeof(InitGroup))]
    [BurstCompile]
    public partial struct FMODInitSoundSystem : ISystem, ISystemStartStop
    {
        private NativeArray<SoundDataEntity> soundDataArrayLocalRef;
        private NativeArray<PARAMETER_DESCRIPTION> soundParamDataArrayLocalRef;

        // SoundData Id / RuntimeIndex
        private NativeHashMap<int, int> soundIdMappingLocalRef;

        private EntityQuery updateQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            updateQuery = SystemAPI.QueryBuilder()
                .WithNone<FMODSound>()
                .WithAll<SoundComponent>()
                .Build();

            state.RequireForUpdate(updateQuery);
            state.RequireForUpdate<FMODSoundDataProviderSystem.InitTag>();
        }

        void ISystem.OnDestroy(ref SystemState state)
        {
            soundDataArrayLocalRef = default;
            soundParamDataArrayLocalRef = default;
            soundIdMappingLocalRef = default;
        }

        void ISystemStartStop.OnStartRunning(ref SystemState state)
        {
            if (!soundDataArrayLocalRef.IsCreated)
            {
                soundDataArrayLocalRef = FMODSoundDataProviderSystem.SoundDataArrayStaticRef;
                soundParamDataArrayLocalRef = FMODSoundDataProviderSystem.SoundParamDataArrayStaticRef;
                soundIdMappingLocalRef = FMODSoundDataProviderSystem.SoundIdMappingStaticRef;
            }
        }

        void ISystemStartStop.OnStopRunning(ref SystemState state) { }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var initializeFMODSoundJob = new InitializeFMODSoundJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
                SoundDataArray = soundDataArrayLocalRef,
                SoundParamDataArray = soundParamDataArrayLocalRef,
                SoundIdMapping = soundIdMappingLocalRef,
                SoundDelayLookup = SystemAPI.GetComponentLookup<SoundDelayData>(true),
                SoundVolumeLookup = SystemAPI.GetComponentLookup<SoundVolume>(true),
                OneShotLookup = SystemAPI.GetComponentLookup<OneShot>(true),
                LocalTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true),
            };

            initializeFMODSoundJob.Schedule();
        }

        [WithNone(typeof(FMODSound))]
        [WithAll(typeof(SoundComponent))]
        [BurstCompile]
        public partial struct InitializeFMODSoundJob : IJobEntity
        {
            public EntityCommandBuffer CommandBuffer;

            [ReadOnly]
            public NativeArray<SoundDataEntity> SoundDataArray;

            [ReadOnly]
            public NativeArray<PARAMETER_DESCRIPTION> SoundParamDataArray;

            [ReadOnly]
            public NativeHashMap<int, int> SoundIdMapping;

            [ReadOnly]
            public ComponentLookup<SoundDelayData> SoundDelayLookup;

            [ReadOnly]
            public ComponentLookup<SoundVolume> SoundVolumeLookup;

            [ReadOnly]
            public ComponentLookup<OneShot> OneShotLookup;

            [ReadOnly]
            public ComponentLookup<LocalTransform> LocalTransformLookup;

            void Execute(
                Entity entity,
                in SoundComponent sound)
            {
                if (!SoundIdMapping.TryGetValue(sound.Id, out var runtimeSoundIndex))
                {
#if UNITY_EDITOR
                    UnityEngine.Debug.Log($"Sound not found id '{sound.Id}'");
#endif
                    return;
                }

                if (SoundDataArray.Length <= runtimeSoundIndex)
                {
#if UNITY_EDITOR
                    UnityEngine.Debug.Log($"Sound data not found runtime index '{runtimeSoundIndex}' Array length {SoundDataArray.Length}");
#endif
                    return;
                }

                var soundData = SoundDataArray[runtimeSoundIndex];

                ref var eventDescription = ref soundData.EventDescription;

                float volume = 1f;

                if (SoundVolumeLookup.HasComponent(entity))
                {
                    volume = SoundVolumeLookup[entity].Volume;
                }

                if (volume == 0)
                {
                    volume = 0.01f;
                }

                var result = eventDescription.createInstance(out var instance);

                bool delayedSound = SoundDelayLookup.HasComponent(entity);

                if (!delayedSound)
                {
                    result = instance.start();
                }

                instance.setVolume(volume);

                if (LocalTransformLookup.HasComponent(entity))
                {
                    var entityPosition = (Vector3)LocalTransformLookup[entity].Position;

                    if (entityPosition != Vector3.zero)
                    {
                        var posAttributes = entityPosition.To3DAttributes();
                        instance.set3DAttributes(posAttributes);
                    }
                }

                if (!delayedSound)
                {
                    if (OneShotLookup.HasComponent(entity))
                    {
                        instance.release();
                        CommandBuffer.DestroyEntity(entity);
                        return;
                    }
                }

                CommandBuffer.AddComponent(entity, new FMODSound()
                {
                    Event = instance
                });

                if (soundData.ParamCount > 0)
                {
                    int localParamIndex = 0;

                    var floatParameters = CommandBuffer.AddBuffer<FMODFloatParameter>(entity);

                    floatParameters.Capacity = soundData.ParamCount;
                    floatParameters.Length = soundData.ParamCount;

                    for (int paramIndex = soundData.StartParamIndex; paramIndex <= soundData.EndParamIndex; paramIndex++)
                    {
                        if (SoundParamDataArray.Length <= paramIndex)
                        {
#if UNITY_EDITOR
                            UnityEngine.Debug.Log($"FMODSoundSystem. Out of fmod param array size. ArraySize {SoundParamDataArray.Length} ParamIndex {paramIndex}");
#endif
                            continue;
                        }

                        var paramDescription = SoundParamDataArray[paramIndex];

                        floatParameters[localParamIndex] = new FMODFloatParameter()
                        {
                            ParameterId = paramDescription.id
                        };

                        localParamIndex++;
                    }
                }
            }
        }
    }
}
#endif