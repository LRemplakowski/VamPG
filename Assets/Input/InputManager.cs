using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class InputManager : ExposableMonobehaviour
{
    //public static PlayerInputMapping actions;
    //public static event Action<InputActionMap> ActionMapChange;

    //private void Awake()
    //{
    //    actions = new PlayerInputMapping();
    //}

    //private void Start()
    //{
    //    ToggleActionMap(actions.MainMenu);
    //}

    //public static void ToggleActionMap(InputActionMap actionMap)
    //{
    //    if (actionMap.enabled)
    //        return;

    //    actions.Disable();
    //    ActionMapChange?.Invoke(actionMap);
    //    actionMap.Enable();
    //}

    private static PlayerInput playerInput;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public static void ToggleActionMap(InputMap inputMap)
    {
        playerInput.SwitchCurrentActionMap(inputMap.ToString());
    }
}

public enum InputMap
{
    Player,
    MainMenu
}
