using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Spirit604.DotsCity.Core.Sound
{
    public class BuiltInSoundService : MonoBehaviour, ISoundService, ISoundPlayer
    {
        [SerializeField] private BuiltInSoundFactory builtInSoundFactory;
        [SerializeField] private BuiltInSoundFactory oneShotSoundFactory;
        [SerializeField] private SoundDataContainer soundDataContainer;
        [SerializeField][Range(0f, 1f)] private float volume = 0.6f;

        private List<AudioSourceBehaviour> allLiveSounds = new List<AudioSourceBehaviour>();
        private List<AudioSourceBehaviour> oneShotSounds = new List<AudioSourceBehaviour>();
        private Dictionary<int, SoundData> allData;
        private Coroutine routine;
        private bool mute;

        public List<SoundData> Sounds => soundDataContainer.Sounds;

        private float CurrentTime => Time.time;

        public void Initialize()
        {
            builtInSoundFactory.Populate();
            oneShotSoundFactory.Populate();

            allData = new Dictionary<int, SoundData>();

            foreach (var soundData in Sounds)
            {
                allData.Add(soundData.Id, soundData);
            }
        }

        public SoundData GetSoundById(int id)
        {
            if (allData.TryGetValue(id, out var soundData)) return soundData;

            UnityEngine.Debug.LogError($"BuiltInSoundService. Sound id {id} doesn't exist.");

            return null;
        }

        public void Mute()
        {
            mute = true;

            for (int i = 0; i < allLiveSounds.Count; i++)
            {
                allLiveSounds[i].SetMute(mute);
            }
        }

        public void Unmute()
        {
            mute = false;

            for (int i = 0; i < allLiveSounds.Count; i++)
            {
                allLiveSounds[i].SetMute(mute);
            }
        }

        public AudioSourceBehaviour GetSound(int id, bool autoPlay = true)
        {
            var soundData = GetSoundById(id);
            AudioSourceBehaviour soundBehaviour = GetSound();

            if (soundBehaviour.SetSound(soundData, autoPlay, soundData.Loop))
            {
                allLiveSounds.Add(soundBehaviour);
                soundBehaviour.OnDisabled += SoundBehaviour_OnDisabled;
                return soundBehaviour;
            }
            else
            {
                soundBehaviour.Release();
            }

            return null;
        }

        public void PlayOneShot(int id, Vector3 position, float delay = 0)
        {
            var soundData = GetSoundById(id);

            if (soundData)
                PlayOneShot(soundData, position);
        }

        public void PlayOneShot(SoundData soundData, Vector3 position)
        {
            AudioSourceBehaviour soundBehaviour = GetSound(true);

            if (soundBehaviour.PlayOneShot(soundData, position))
            {
                allLiveSounds.Add(soundBehaviour);
                soundBehaviour.DisableTime = CurrentTime + soundBehaviour.Length;
                oneShotSounds.Add(soundBehaviour);
                StartUpdate();
            }
            else
            {
                soundBehaviour.Release();
            }
        }

        public void ChangeVolume(float volume)
        {
            Assert.IsTrue(volume >= 0, "Negative value of the sound volume.");
            this.volume = volume;

            for (int i = 0; i < allLiveSounds.Count; i++)
            {
                allLiveSounds[i].BaseVolume = volume;
            }
        }

        private AudioSourceBehaviour GetSound(bool oneshot = false)
        {
            AudioSourceBehaviour sound = null;

            if (!oneshot)
            {
                sound = builtInSoundFactory.Get();
            }
            else
            {
                sound = oneShotSoundFactory.Get();
            }

            sound.SetMute(mute);
            sound.BaseVolume = volume;
            return sound;
        }

        private void StartUpdate()
        {
            if (routine == null)
            {
                routine = StartCoroutine(Tick());
            }
        }

        private IEnumerator Tick()
        {
            while (true)
            {
                int index = 0;

                while (index < oneShotSounds.Count)
                {
                    if (CurrentTime > oneShotSounds[index].DisableTime)
                    {
                        allLiveSounds.Remove(oneShotSounds[index]);
                        oneShotSounds[index].Release();
                        oneShotSounds.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }

                if (oneShotSounds.Count == 0)
                {
                    routine = null;
                    break;
                }

                yield return null;
            }
        }

        private void SoundBehaviour_OnDisabled(AudioSourceBehaviour audioSource)
        {
            audioSource.OnDisabled -= SoundBehaviour_OnDisabled;
            allLiveSounds.Remove(audioSource);
            audioSource.Release(false);
        }
    }
}
