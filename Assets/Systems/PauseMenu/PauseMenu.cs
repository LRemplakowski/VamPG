using UnityEngine;

namespace SunsetSystems.UI.Pause
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField]
        private Canvas _pauseMenuCanvas;
        [SerializeField]
        private GUIWindowsManager _windowsManager;
        [SerializeField]
        private PauseUISelector _inventory, _journal, _settings;
        public static bool IsGamePaused { get; private set; }
        private GameState _cachedPreviousState;

        private void Awake()
        {
            if (_pauseMenuCanvas == null)
                _pauseMenuCanvas = GetComponent<Canvas>();
            if (_windowsManager == null)
                _windowsManager = FindObjectOfType<GUIWindowsManager>();
            StateManager.OnGameStateChanged += CachePreviousState;
        }

        private void OnDestroy()
        {
            StateManager.OnGameStateChanged -= CachePreviousState;
        }

        private void CachePreviousState(GameState newState, GameState previousState)
        {
            _cachedPreviousState = previousState;
        }

        public void PauseGame()
        {
            HandleOpenPauseGUI();
            OpenMenuScreen(PauseMenuScreen.Settings);
        }

        public void OpenInventoryScreen()
        {
            HandleOpenPauseGUI();
            OpenMenuScreen(PauseMenuScreen.Inventory);
        }

        public void OpenJournalScreen()
        {
            HandleOpenPauseGUI();
            OpenMenuScreen(PauseMenuScreen.Journal);
        }

        private void OpenMenuScreen(PauseMenuScreen screen)
        {
            switch (screen)
            {
                case PauseMenuScreen.Settings:
                    _settings.SelectGUIScreen();
                    break;
                case PauseMenuScreen.Inventory:
                    _inventory.SelectGUIScreen();
                    break;
                case PauseMenuScreen.CharacterSheet:
                    break;
                case PauseMenuScreen.Journal:
                    _journal.SelectGUIScreen();
                    break;
                case PauseMenuScreen.Map:
                    break;
            }
        }

        private void HandleOpenPauseGUI()
        {
            IsGamePaused = true;
            gameObject.SetActive(true);
            StateManager.SetCurrentState(GameState.GamePaused);
        }

        public void ResumeGame()
        {
            IsGamePaused = false;
            gameObject.SetActive(false);
            StateManager.SetCurrentState(_cachedPreviousState);
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