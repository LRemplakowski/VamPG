using UnityEngine;
using SunsetSystems.Utils;
using UnityEngine.Audio;

namespace SunsetSystems.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField]
        private AudioMixer _audioMixer;

        public void PlaySFX(AudioClip clip)
        {
            
        }
    }
}
