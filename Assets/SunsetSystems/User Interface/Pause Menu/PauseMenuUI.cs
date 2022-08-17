using SunsetSystems.UI.Pause;
using SunsetSystems.Utils;
using UnityEngine;

namespace SunsetSystems.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField]
        private Canvas pauseMenuCanvas;
        [SerializeField]
        private PauseUISelector inventory, journal, settings, characterSheet;

        private void Awake()
        {
            if (!pauseMenuCanvas)
                pauseMenuCanvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void OpenSettingsScreen()
        {
            OpenMenuScreen(PauseMenuScreen.Settings);
        }

        public void OpenCharacterSheetScreen()
        {
            OpenMenuScreen(PauseMenuScreen.CharacterSheet);
        }

        public void OpenInventoryScreen()
        {
            OpenMenuScreen(PauseMenuScreen.Inventory);
        }

        public void OpenJournalScreen()
        {
            OpenMenuScreen(PauseMenuScreen.Journal);
        }

        private void OpenMenuScreen(PauseMenuScreen screen)
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