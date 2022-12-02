using SunsetSystems.Audio;
using SunsetSystems.Constants;
using SunsetSystems.Data;
using SunsetSystems.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.MainMenu
{
    [RequireComponent(typeof(Tagger))]
    public class MainMenuUIManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject debugUi;
        [SerializeField]
        private Slider _musicSlider, _sfxSlider;

        // Start is called before the first frame update
        void Start()
        {
            if (debugUi)
                debugUi.SetActive(GameConstants.DEBUG_MODE);
            if (PlayerPrefs.HasKey("MUSIC_VOLUME"))
                _musicSlider.value = PlayerPrefs.GetFloat("MUSIC_VOLUME");
            if (PlayerPrefs.HasKey("SFX_VOLUME"))
                _sfxSlider.value = PlayerPrefs.GetFloat("SFX_VOLUME");
            _musicSlider.onValueChanged.AddListener(SignalMusicVolumeChange);
            _sfxSlider.onValueChanged.AddListener(SignalSFXVolumeChange);
        }

        public void StartGameDebug()
        {
            GameStarter.Instance.InitializeGameDebug();
        }

        public void StartGameJam()
        {
            GameStarter.Instance.InitializeGameJam();
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void SignalMusicVolumeChange(float volume)
        {
            AudioManager.Instance.SetMusicVolume(volume);
        }

        public void SignalSFXVolumeChange(float volume)
        {
            AudioManager.Instance.SetSFXVolume(volume);
        }
    }
}
