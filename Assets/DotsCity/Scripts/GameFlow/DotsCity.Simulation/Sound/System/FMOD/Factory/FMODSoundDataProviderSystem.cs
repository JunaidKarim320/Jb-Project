#if FMOD
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Spirit604.DotsCity.Core.Sound;
using Unity.Collections;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Sound
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class FMODSoundDataProviderSystem : SystemBase
    {
        public struct InitTag : IComponentData { }

        private NativeArray<SoundDataEntity> soundDataArray;
        private NativeArray<PARAMETER_DESCRIPTION> soundParamDataArray;
        private NativeHashMap<int, int> soundIdMapping;

        public static NativeArray<SoundDataEntity> SoundDataArrayStaticRef { get; private set; }
        public static NativeArray<PARAMETER_DESCRIPTION> SoundParamDataArrayStaticRef { get; private set; }
        public static NativeHashMap<int, int> SoundIdMappingStaticRef { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            Enabled = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (soundDataArray.IsCreated)
            {
                soundDataArray.Dispose();
                SoundDataArrayStaticRef = default;
            }

            if (soundParamDataArray.IsCreated)
            {
                soundParamDataArray.Dispose();
                SoundParamDataArrayStaticRef = default;
            }

            if (soundIdMapping.IsCreated)
            {
                soundIdMapping.Dispose();
                SoundIdMappingStaticRef = default;
            }
        }

        protected override void OnUpdate() { }

        public void Initialize(ISoundService fmodSoundService)
        {
            var sounds = fmodSoundService.Sounds;

            int paramIndex = 0;

            if (sounds != null)
            {
                soundDataArray = new NativeArray<SoundDataEntity>(sounds.Count, Allocator.Persistent);
                soundIdMapping = new NativeHashMap<int, int>(sounds.Count, Allocator.Persistent);

                SoundDataArrayStaticRef = soundDataArray;
                SoundIdMappingStaticRef = soundIdMapping;

                var tempParamList = new NativeList<PARAMETER_DESCRIPTION>(Allocator.TempJob);

                for (int i = 0; i < sounds.Count; i++)
                {
                    var soundData = sounds[i];

                    var result = RuntimeManager.StudioSystem.getEvent(soundData.Name, out var eventDescription);

                    var data = soundDataArray[i];
                    data.EventDescription = eventDescription;

                    if (!soundIdMapping.ContainsKey(soundData.Id))
                    {
                        soundIdMapping.Add(soundData.Id, i);
                    }
                    else
                    {
                        UnityEngine.Debug.LogError($"FMODSoundDataProviderSystem. Sound {soundData.Name} Duplicate id {soundData.Id}");
                        continue;
                    }

                    if (result != RESULT.OK)
                    {
                        UnityEngine.Debug.LogError($"FMODSoundDataProviderSystem. Event {soundData.Name} not found");
                    }

                    if (soundData.Parameters?.Length > 0)
                    {
                        data.StartParamIndex = paramIndex;

                        for (int j = 0; j < soundData.Parameters.Length; j++)
                        {
                            result = data.EventDescription.getParameterDescriptionByName(soundData.Parameters[j].Name, out var parameterDescription);

                            if (result != RESULT.OK)
                            {
                                UnityEngine.Debug.LogError($"FMODSoundDataProviderSystem. Event {soundData.Name} parameter {soundData.Parameters[j].Name} not found");
                            }

                            tempParamList.Add(parameterDescription);
                            paramIndex++;
                        }

                        data.EndParamIndex = paramIndex - 1;
                    }
                    else
                    {
                        data.StartParamIndex = -1;
                        data.EndParamIndex = -1;
                    }

                    soundDataArray[i] = data;
                }

                soundParamDataArray = tempParamList.ToArray(Allocator.Persistent);
                SoundParamDataArrayStaticRef = soundParamDataArray;
                tempParamList.Dispose();
            }

            EntityManager.CreateEntity(typeof(FMODSoundDataProviderSystem.InitTag));
        }
    }
}
#endif