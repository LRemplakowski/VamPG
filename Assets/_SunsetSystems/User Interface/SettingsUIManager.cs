using SunsetSystems.Audio;
using SunsetSystems.Core;
using SunsetSystems.Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems
{
    public class SettingsUIManager : MonoBehaviour
    {
        [SerializeField]
        private Slider _musicSlider, _sfxSlider, _typewriterSlider;

        private void OnEnable()
        {
            if (PlayerPrefs.HasKey(SettingsConstants.MUSIC_VOLUME_KEY))
                _musicSlider.value = PlayerPrefs.GetFloat(SettingsConstants.MUSIC_VOLUME_KEY);
            else
                _musicSlider.value = AudioManager.Instance.MusicDefaultValue;
            if (PlayerPrefs.HasKey(SettingsConstants.SFX_VOLUME_KEY))
                _sfxSlider.value = PlayerPrefs.GetFloat(SettingsConstants.SFX_VOLUME_KEY);
            else
                _sfxSlider.value = AudioManager.Instance.SFXDefaultValue;
            if (PlayerPrefs.HasKey(SettingsConstants.TYPEWRITER_SPEED_KEY))
                _typewriterSlider.value = PlayerPrefs.GetInt(SettingsConstants.TYPEWRITER_SPEED_KEY);
            else
                _typewriterSlider.value = DialogueManager.Instance.DefaultTypewriterValue;
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
            AudioManager.Instance.SetMusicVolume(volume);
        }

        public void SignalSFXVolumeChange(float volume)
        {
            AudioManager.Instance.SetSFXVolume(volume);
        }

        private void SignalTypewriterSpeedChange(float speed)
        {
            DialogueManager.Instance.SetTypewriterSpeed(speed);
        }
    }
}
