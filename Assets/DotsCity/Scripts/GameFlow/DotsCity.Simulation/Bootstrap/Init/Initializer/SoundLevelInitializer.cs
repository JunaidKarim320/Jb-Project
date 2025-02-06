using Spirit604.Attributes;
using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Core.Bootstrap;
using Spirit604.DotsCity.Core.Sound;
using Spirit604.DotsCity.Hybrid.Core;
using Spirit604.DotsCity.Simulation.Car.Sound;
using Spirit604.DotsCity.Simulation.Sound;
using Spirit604.DotsCity.Simulation.Sound.Pedestrian;
using Spirit604.DotsCity.Simulation.Traffic.Sound;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Initialization
{
    public class SoundLevelInitializer : InitializerBase, IPreInitializer, ILateInitializer
    {
#if !FMOD
        private BuiltInSoundService builtInSoundService;
#endif

        private ISoundService soundService;
        private SoundLevelConfig soundLevelConfig;

        [InjectWrapper]
        public void Construct(
            ISoundService soundService,
            SoundLevelConfig soundLevelConfig
#if !FMOD   
            ,
            BuiltInSoundService builtInSoundService = null
#endif
            )
        {
#if !FMOD
            this.builtInSoundService = builtInSoundService;
#endif

            this.soundService = soundService;
            this.soundLevelConfig = soundLevelConfig;
        }

        public override void Initialize()
        {
            base.Initialize();

            if (soundLevelConfig.HasSounds)
            {
                InitSound();
                InitSoundSettings();
                CreateAudioListener();
            }
            else
            {
                DisableSoundSystems();
            }
        }

        private void InitSound()
        {
            soundService.Initialize();

#if FMOD
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<FMODSoundDataProviderSystem>().Initialize(soundService);
#else
            if (builtInSoundService && soundLevelConfig.HasSounds)
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<UnitySoundInitSystem>().Initialize(builtInSoundService);
#endif
        }

        private void InitSoundSettings()
        {
            if (!soundLevelConfig.CrowdSound)
            {
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<CrowdSoundSystem>(false);
            }

            if (!soundLevelConfig.RandomHornsSound)
            {
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficHornSoundSystem>(false);
            }
        }

        private void DisableSoundSystems()
        {
            DefaultWorldUtils.SwitchActiveUnmanagedSystem<CrowdSoundSystem>(false);

            DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficHornSoundSystem>(false);
            DefaultWorldUtils.SwitchActiveUnmanagedSystem<CarCullSoundSystem1>(false);
            DefaultWorldUtils.SwitchActiveUnmanagedSystem<CarCullSoundSystem2>(false);
            DefaultWorldUtils.SwitchActiveUnmanagedSystem<CarCullSoundSystem3>(false);
            DefaultWorldUtils.SwitchActiveUnmanagedSystem<CarSoundSystem>(false);
            DefaultWorldUtils.SwitchActiveUnmanagedSystem<CarUpdateSoundSystem>(false);
            DefaultWorldUtils.SwitchActiveUnmanagedSystem<SoundLoopSystem>(false);

#if FMOD
            DefaultWorldUtils.SwitchActiveUnmanagedSystem<FMODInitSoundSystem>(false);
#endif
        }

        private void CreateAudioListener()
        {
            if (!soundLevelConfig.CustomAudioListener)
                return;

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var entity = entityManager.CreateEntity(
                typeof(CopyTransformToGameObject),
                typeof(PlayerTrackerTag),
                typeof(LocalTransform),
                typeof(LocalToWorld),
                typeof(Transform));

            var audioListener = new GameObject("AudioListener");

#if FMOD
            audioListener.AddComponent<FMODUnity.StudioListener>();
#else
            audioListener.AddComponent<AudioListener>();
#endif

            entityManager.AddComponentObject(entity, audioListener.transform);
        }

        public void PreInitialize()
        {
            MuteSound();
        }

        public void LateInitialize()
        {
            EnableSound();
        }

        private void MuteSound()
        {
            soundService.Mute();
        }

        private void EnableSound()
        {
            if (soundLevelConfig.HasSounds)
            {
                soundService.Unmute();
            }
        }
    }
}