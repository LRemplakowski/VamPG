using SunsetSystems.LevelManagement;
using SunsetSystems.LevelManagement.UI;
using SunsetSystems.UI.Pause;
using SunsetSystems.Utils;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

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

        public UnityEvent OnReturnToMenu;
        
        public PauseMenuScreen CurrentActiveScreen { get; private set; }

        private ILevelLoader _levelLoader;

        [Inject]
        public void InjectDependencies(ILevelLoader levelLoader)
        {
            _levelLoader = levelLoader;
        }

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

        public void QuitToMenu()
        {
            //await _levelLoader.UnloadGameScene();
            Debug.LogError("Rework back to menu dumbass!");
            OnReturnToMenu?.Invoke();
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