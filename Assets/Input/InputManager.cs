using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class InputManager : ExposableMonobehaviour
{
    private static PlayerInput _playerInput;

    private void Awake()
    {
        StateManager.OnGameStateChanged += SwitchInputOnStateChange;
    }

    private void OnDestroy()
    {
        StateManager.OnGameStateChanged -= SwitchInputOnStateChange;
    }

    private void SwitchInputOnStateChange(GameState newState, GameState previousState)
    {
        switch (newState)
        {
            case GameState.GamePaused:
                ToggleActionMap(InputMap.UI);
                break;
            case GameState.MainMenu:
                ToggleActionMap(InputMap.UI);
                break;
            default:
                ToggleActionMap(InputMap.Player);
                break;
        }
    }

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    public static void ToggleActionMap(InputMap inputMap)
    {
        _playerInput.SwitchCurrentActionMap(inputMap.ToString());        
    }
}

public enum InputMap
{
    Player,
    UI
}
