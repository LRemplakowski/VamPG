using SunsetSystems.Core.SceneLoading.UI;
using SunsetSystems.UI.Pause;
using UnityEngine;
using UnityEngine.Events;

namespace SunsetSystems.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField]
        private Canvas pauseMenuCanvas;
        [SerializeField]
        private PauseUISelector inventory, journal, settings, characterSheet;
        [SerializeField]
        private GameObject _characterSelector;

        [SerializeField]
        private UnityEvent OnReturnToMenu;
        
        public PauseMenuScreen CurrentActiveScreen { get; private set; }

        private void Awake()
        {
            if (!pauseMenuCanvas)
                pauseMenuCanvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            gameObject.SetActive(false);
            _characterSelector.SetActive(true);
            _characterSelector.SetActive(false);
        }

        public async void QuitToMenu()
        {
            SceneLoadingUIManager loading = this.FindFirstComponentWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);
            await loading.DoFadeOutAsync(.5f);
            //await LevelLoader.Instance.UnloadGameScene();
            OnReturnToMenu?.Invoke();
            await loading.DoFadeInAsync(.5f);
        }

        public void OpenMenuScreen(PauseMenuScreen screen)
        {
            switch (screen)
            {
                case PauseMenuScreen.Settings:
                    settings.SelectGUIScreen();
                    break;
                case PauseMenuScreen.Inventory:
                    inventory.SelectGUIScreen();
                    break;
                case PauseMenuScreen.CharacterSheet:
                    characterSheet.SelectGUIScreen();
                    break;
                case PauseMenuScreen.Journal:
                    journal.SelectGUIScreen();
                    break;
                case PauseMenuScreen.Map:
                    break;
            }
            CurrentActiveScreen = screen;
        }
    }

    public enum PauseMenuScreen
    {
        Settings,
        Inventory,
        CharacterSheet,
        Journal,
        Map
    }
}