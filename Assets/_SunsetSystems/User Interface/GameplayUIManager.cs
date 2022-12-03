using SunsetSystems.Audio;
using SunsetSystems.Entities;
using SunsetSystems.Inventory.UI;
using SunsetSystems.UI.Pause;
using SunsetSystems.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace SunsetSystems.UI
{
    [RequireComponent(typeof(Tagger))]
    public class GameplayUIManager : MonoBehaviour
    {
        [field: SerializeField]
        public InGameUI InGameUI { get; private set; }
        [field: SerializeField]
        public PauseMenuUI PauseMenuUI { get; private set; }
        [field: SerializeField]
        public ContainerGUI ContainerGUI { get; private set; }
        [field: SerializeField]
        public HoverNameplate HoverNameplate { get; private set; }
        [field: SerializeField]
        public DialogueViewBase DialogueGUI { get; private set; }
        [field: SerializeField]
        public GameObject HelpOverlay { get; private set; }
        [SerializeField]
        private Slider _musicSlider, _sfxSlider;

        private void OnEnable()
        {
            if (PlayerPrefs.HasKey("MUSIC_VOLUME"))
                _musicSlider.value = PlayerPrefs.GetFloat("MUSIC_VOLUME");
            if (PlayerPrefs.HasKey("SFX_VOLUME"))
                _sfxSlider.value = PlayerPrefs.GetFloat("SFX_VOLUME");
            _musicSlider.onValueChanged.AddListener(SignalMusicVolumeChange);
            _sfxSlider.onValueChanged.AddListener(SignalSFXVolumeChange);
        }

        private void OnDisable()
        {
            _musicSlider.onValueChanged.AddListener(SignalMusicVolumeChange);
            _sfxSlider.onValueChanged.AddListener(SignalSFXVolumeChange);
        }

        public void HandleNameplateHover(INameplateReciever nameplateReciever)
        {
            if (string.IsNullOrEmpty(nameplateReciever.NameplateText))
            {
                DisableNameplate();
                return;
            }
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(nameplateReciever.NameplateWorldPosition);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(InGameUI.transform as RectTransform, screenPoint, Camera.main, out Vector2 nameplatePosition);
            HoverNameplate.transform.position = screenPoint;
            HoverNameplate.SetNameplateText(nameplateReciever.NameplateText);
            HoverNameplate.gameObject.SetActive(true);
        }

        public void DisableNameplate()
        {
            HoverNameplate.gameObject.SetActive(false);
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
