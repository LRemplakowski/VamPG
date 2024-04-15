using Sirenix.OdinInspector;
using SunsetSystems.Core.SceneLoading;
using SunsetSystems.UI.Pause;
using UltEvents;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.UI
{
    public class PauseMenuScreenHandler : SerializedMonoBehaviour
    {
        [SerializeField]
        private PauseUISelector inventory, journal, settings, characterSheet;
        [SerializeField]
        private GameObject _characterSelector;
        [SerializeField]
        private GameObject _saveLoadScreen;

        [SerializeField]
        private UltEvent _onReturnToMenu;
        
        public PauseMenuScreen CurrentActiveScreen { get; private set; }

        private void Start()
        {
            _characterSelector.SetActive(true);
            _characterSelector.SetActive(false);
        }

        public void QuitToMenu()
        {
            _onReturnToMenu?.Invoke();
            _ = LevelLoader.Instance.BackToMainMenu();
        }

        public void OpenMenuScreen(PauseMenuScreen screen)
        {
            switch (screen)
            {
                case PauseMenuScreen.Settings:
                    gameObject.SetActive(true);
                    _characterSelector.SetActive(false);
                    settings.SelectGUIScreen();
                    break;
                case PauseMenuScreen.Inventory:
                    gameObject.SetActive(true);
                    _characterSelector.SetActive(true);
                    inventory.SelectGUIScreen();
                    break;
                case PauseMenuScreen.CharacterSheet:
                    gameObject.SetActive(true);
                    _characterSelector.SetActive(true);
                    characterSheet.SelectGUIScreen();
                    break;
                case PauseMenuScreen.Journal:
                    gameObject.SetActive(true);
                    _characterSelector.SetActive(false);
                    journal.SelectGUIScreen();
                    break;
                case PauseMenuScreen.Map:
                    break;
                case PauseMenuScreen.None:
                    gameObject.SetActive(false);
                    break;
            }
            CurrentActiveScreen = screen;
        }

        public void OnCancelAction(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (gameObject.activeInHierarchy is true && _saveLoadScreen.activeInHierarchy is false)
                {
                    OpenMenuScreen(PauseMenuScreen.None);
                }
                else
                {
                    _saveLoadScreen.SetActive(false);
                    OpenMenuScreen(PauseMenuScreen.Settings);
                }
            }
        }

        public void OnCharacterSheetAction(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (CurrentActiveScreen == PauseMenuScreen.CharacterSheet)
                {
                    OpenMenuScreen(PauseMenuScreen.None);
                }
                else
                {
                    OpenMenuScreen(PauseMenuScreen.CharacterSheet);
                }
            }
        }

        public void OnInventoryAction(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (CurrentActiveScreen == PauseMenuScreen.Inventory)
                {
                    OpenMenuScreen(PauseMenuScreen.None);
                }
                else
                {
                    OpenMenuScreen(PauseMenuScreen.Inventory);
                }
            }
        }

        public void OnJournalAction(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (CurrentActiveScreen == PauseMenuScreen.Journal)
                {
                    OpenMenuScreen(PauseMenuScreen.None);
                }
                else
                {
                    OpenMenuScreen(PauseMenuScreen.Journal);
                }
            }
        }
    }

    public enum PauseMenuScreen
    {
        Settings,
        Inventory,
        CharacterSheet,
        Journal,
        Map,
        None
    }
}