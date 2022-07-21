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
    [SerializeField]
    private InputMap _inputMap = InputMap.UI;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        StateManager.OnGameStateChanged += SwitchInputOnStateChange;
    }

    private void OnDestroy()
    {
        StateManager.OnGameStateChanged -= SwitchInputOnStateChange;
    }

    private void Update()
    {
        if (_inputMap != _currentInputMap)
            ToggleActionMap(_inputMap);
    }

    private void SwitchInputOnStateChange(GameState newState, GameState previousState)
    {
        switch (newState)
        {
            case GameState.GamePaused:
                ToggleActionMap(InputMap.UI);
                _inputMap = _currentInputMap;
                break;
            case GameState.Menu:
                ToggleActionMap(InputMap.UI);
                _inputMap = _currentInputMap;
                break;
            case GameState.Exploration:
                ToggleActionMap(InputMap.Player);
                _inputMap = _currentInputMap;
                break;
            case GameState.Combat:
                ToggleActionMap(InputMap.Player);
                _inputMap = _currentInputMap;
                break;
            case GameState.Conversation:
                ToggleActionMap(InputMap.UI);
                _inputMap = _currentInputMap;
                break;
            default:
                ToggleActionMap(InputMap.UI);
                _inputMap = _currentInputMap;
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
