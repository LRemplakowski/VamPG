using SunsetSystems.Audio;
using SunsetSystems.Constants;
using SunsetSystems.Dialogue;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SunsetSystems
{
    public class SettingsUInó : MonoBehaviour
    {
        [SerializeField]
        private Slider _musicSlider, _sfxSlider, _typewriterSlider;

        private IAudioManager _audioManager;
        private IDialogueManager _dialogueManager;

        [Inject]
        public void InjectDependencies(IAudioManager audioManager, IDialogueManager dialogueManager)
        {
            _audioManager = audioManager;
            _dialogueManager = dialogueManager;
        }

        private void OnEnable()
        {
            if (PlayerPrefs.HasKey(SettingsConstants.MUSIC_VOLUME_KEY))
                _musicSlider.value = PlayerPrefs.GetFloat(SettingsConstants.MUSIC_VOLUME_KEY);
            else
                _musicSlider.value = _audioManager.MusicDefaultVolume;
            if (PlayerPrefs.HasKey(SettingsConstants.SFX_VOLUME_KEY))
                _sfxSlider.value = PlayerPrefs.GetFloat(SettingsConstants.SFX_VOLUME_KEY);
            else
                _sfxSlider.value = _audioManager.SFXDefaultVolume;
            if (PlayerPrefs.HasKey(SettingsConstants.TYPEWRITER_SPEED_KEY))
                _typewriterSlider.value = PlayerPrefs.GetInt(SettingsConstants.TYPEWRITER_SPEED_KEY);
            else
                _typewriterSlider.value = _dialogueManager.DefaultTypewriterSpeed;
            _musicSlider.onValueChanged.AddListener(SignalMusicVolumeChange);
            _sfxSlider.onValueChanged.AddListener(SignalSFXVolumeChange);
            _typewriterSlider.onValueChanged.AddListener(SignalTypewriterSpeedChange);
        }

        private void OnDisable()
        {
            _musicSlider.onValueChanged.RemoveListener(SignalMusicVolumeChange);
            _sfxSlider.onValueChanged.RemoveListener(SignalSFXVolumeChange);
            _typewriterSlider.onValueChanged.RemoveListener(SignalTypewriterSpeedChange);
        }

        public void SignalMusicVolumeChange(float volume)
        {
            _audioManager.SetMusicVolume(volume);
        }

        public void SignalSFXVolumeChange(float volume)
        {
            _audioManager.SetSFXVolume(volume);
        }

        private void SignalTypewriterSpeedChange(float speed)
        {
            _dialogueManager.SetTypewriterSpeed(speed);
        }
    }
}
