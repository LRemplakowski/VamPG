using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.UI.Pause
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField]
        private Canvas pauseMenuCanvas;
        [SerializeField]
        private GUIWindowsManager windowsManager;
        [SerializeField]
        private PauseUISelector inventory, journal, settings;
        public static bool IsGamePaused { get; private set; }
        private GameState cachedPreviousState;

        private void Awake()
        {
            if (pauseMenuCanvas == null)
                pauseMenuCanvas = GetComponent<Canvas>();
            if (windowsManager == null)
                windowsManager = FindObjectOfType<GUIWindowsManager>();
            StateManager.OnGameStateChanged += CachePreviousState;
        }

        private void OnDestroy()
        {
            StateManager.OnGameStateChanged -= CachePreviousState;
        }

        private void CachePreviousState(GameState newState, GameState previousState)
        {
            cachedPreviousState = previousState;
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
                    settings.SelectGUIScreen();
                    break;
                case PauseMenuScreen.Inventory:
                    inventory.SelectGUIScreen();
                    break;
                case PauseMenuScreen.CharacterSheet:
                    break;
                case PauseMenuScreen.Journal:
                    journal.SelectGUIScreen();
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
            StateManager.SetCurrentState(cachedPreviousState);
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