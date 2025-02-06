using UnityEngine;

namespace Spirit604.DotsCity.Core.Sound
{
    public interface ISoundPlayer
    {
        void PlayOneShot(SoundData soundData, Vector3 position);
    }
}