using UnityEngine;
using SunsetSystems.Utils;
using UnityEngine.Audio;

namespace SunsetSystems.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField]
        private AudioMixer _audioMixer;
        [SerializeField]
        private SFXController _sfxController;
        [SerializeField]
        private SoundtrackController _soundtrackController;

        public void PlaySFX(string sfxName)
        {
            _sfxController.PlayLooped(sfxName);
        }

        public void PlaySFXOneShot(string sfxName)
        {
            _sfxController.PlayOneShot(sfxName);
        }

        public void PlayTyperwriterLoop()
        {
            _sfxController.PlayTyperwriterLoop();
        }

        public void PlayTypewriterEnd()
        {
            _sfxController.StopTyperwriterLoop();
        }

        public void StopSFXPlayback()
        {
            _sfxController.StopSFX();
        }
    }
}
