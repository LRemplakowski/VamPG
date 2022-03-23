using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class InputManager : ExposableMonobehaviour
{
    private static PlayerInput _playerInput;
    private static InputMap _currentInputMap;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
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
            case GameState.Menu:
                ToggleActionMap(InputMap.UI);
                break;
            case GameState.Exploration:
                ToggleActionMap(InputMap.Player);
                break;
            case GameState.Combat:
                ToggleActionMap(InputMap.Player);
                break;
            case GameState.Conversation:
                ToggleActionMap(InputMap.UI);
                break;
            default:
                ToggleActionMap(InputMap.UI);
                break;
        }
    }

    public static void ToggleActionMap(InputMap newInputMap)
    {
        Debug.Log("Switching input map to: " + newInputMap.ToString());
        _playerInput.SwitchCurrentActionMap(newInputMap.ToString());
        _currentInputMap = newInputMap;
    }
}

public enum InputMap
{
    Player,
    UI
}
