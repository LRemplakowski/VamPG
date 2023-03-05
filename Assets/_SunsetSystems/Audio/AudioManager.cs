using UnityEngine;
using SunsetSystems.Utils;
using UnityEngine.Audio;
using SunsetSystems.Constants;

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

        [field: SerializeField]
        public float MusicDefaultValue { get; private set; } = .5f;
        [field: SerializeField]
        public float SFXDefaultValue { get; private set; } = .5f;

        private void Start()
        {
            if (PlayerPrefs.HasKey(SettingsConstants.MUSIC_VOLUME_KEY))
                SetMusicVolume(PlayerPrefs.GetFloat(SettingsConstants.MUSIC_VOLUME_KEY));
            if (PlayerPrefs.HasKey(SettingsConstants.SFX_VOLUME_KEY))
                SetSFXVolume(PlayerPrefs.GetFloat(SettingsConstants.SFX_VOLUME_KEY));
            PlayMenuMusic();
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
            PlayerPrefs.SetFloat(SettingsConstants.MUSIC_VOLUME_KEY, volume);
            PlayerPrefs.Save();
            _soundtrackController.Volume = volume;
        }

        public void SetSFXVolume(float volume)
        {
            PlayerPrefs.SetFloat(SettingsConstants.SFX_VOLUME_KEY, volume);
            PlayerPrefs.Save();
            _sfxController.Volume = volume;
        }
    }
}
