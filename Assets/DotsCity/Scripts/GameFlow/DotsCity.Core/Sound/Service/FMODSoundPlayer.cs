#if FMOD
using UnityEngine;

namespace Spirit604.DotsCity.Core.Sound
{
    public class FMODSoundPlayer : ISoundPlayer
    {
        private IFMODSoundService fMODSoundService;

        public FMODSoundPlayer(IFMODSoundService fMODSoundService)
        {
            this.fMODSoundService = fMODSoundService;
        }

        public void PlayOneShot(SoundData soundData, Vector3 position)
        {
            FMODUnity.RuntimeManager.PlayOneShot(soundData.Name, position);
        }
    }
}
#endif