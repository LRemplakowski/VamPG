using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private Canvas pauseMenuCanvas;
    [SerializeField]
    private GUIWindowsManager windowsManager;
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
        if (windowsManager.ActiveWindows <= 0)
        {
            Time.timeScale = 0;
            IsGamePaused = true;
            gameObject.SetActive(true);
            StateManager.SetCurrentState(GameState.GamePaused);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        IsGamePaused = false;
        gameObject.SetActive(false);
        StateManager.SetCurrentState(cachedPreviousState);
    }
}
