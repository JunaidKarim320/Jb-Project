using Spirit604.CityEditor;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Sound
{
    [CreateAssetMenu(fileName = "SoundLevelConfig", menuName = CityEditorBookmarks.CITY_EDITOR_LEVEL_CONFIG_OTHER_PATH + "SoundLevelConfig")]
    public class SoundLevelConfig : ScriptableObject
    {
        [SerializeField] private bool hasSounds = true;

        [Tooltip("Custom audio listener will follow the player")]
        [SerializeField] private bool customAudioListener = true;

        [SerializeField] private bool crowdSound = true;
        [SerializeField] private bool randomHornsSound = true;

        public bool HasSounds => hasSounds;
        public bool CustomAudioListener => customAudioListener;
        public bool CrowdSound => crowdSound;
        public bool RandomHornsSound => randomHornsSound;
    }
}
