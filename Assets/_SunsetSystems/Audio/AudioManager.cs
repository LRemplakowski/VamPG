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

        private void Start()
        {
            PlayMenuMusic();
            SetMusicVolume(PlayerPrefs.GetFloat("MUSIC_VOLUME"));
            SetSFXVolume(PlayerPrefs.GetFloat("SFX_VOLUME"));
        }

        public void PlaySFXOneShot(string sfxName)
        {
            _sfxController.PlayOneShot(sfxName);
        }

        public void PlaySFXOneShot(AudioClip clip)
        {
            _sfxController.PlayOneShot(clip);
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

        public void PlayMenuMusic()
        {
            _soundtrackController.PlayMenuPlaylist();
        }

        public void PlayGameplayMusic()
        {
            _soundtrackController.PlayGamePlaylist();
        }

        public void SetMusicVolume(float volume)
        {
            PlayerPrefs.SetFloat("MUSIC_VOLUME", volume);
            _soundtrackController.Volume = volume;
        }

        public void SetSFXVolume(float volume)
        {
            PlayerPrefs.SetFloat("SFX_VOLUME", volume);
            _sfxController.Volume = volume;
        }
    }
}
