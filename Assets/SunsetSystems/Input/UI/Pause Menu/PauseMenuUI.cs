using SunsetSystems.UI.Pause;
using UnityEngine;

namespace SunsetSystems.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField]
        private Canvas _pauseMenuCanvas;
        [SerializeField]
        private PauseUISelector _inventory, _journal, _settings;

        private void Awake()
        {
            if (_pauseMenuCanvas == null)
                _pauseMenuCanvas = GetComponent<Canvas>();
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
            gameObject.SetActive(true);
        }

        public void ResumeGame()
        {
            gameObject.SetActive(false);
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