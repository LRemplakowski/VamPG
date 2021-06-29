using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class DevStateChanger : ExposableMonobehaviour
{
    private Dropdown dropdown;

    private void OnEnable()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (int g in Enum.GetValues(typeof(GameState)))
        {
            options.Add(new Dropdown.OptionData(((GameState)g).ToString()));
        }
        dropdown.AddOptions(options);
        dropdown.SetValueWithoutNotify((int)StateManager.instance.GetCurrentState());
        StateManager.instance.onGameStateChanged += OnStateChange;
    }

    private void OnDisable()
    {
        StateManager.instance.onGameStateChanged -= OnStateChange;
    }

    private void OnStateChange(GameState newState, GameState oldState)
    {
        if (dropdown.value != (int)newState)
            UpdateDisplayedState(newState);
    }

    private void UpdateDisplayedState(GameState state)
    {
        dropdown.SetValueWithoutNotify((int)state);
    }
}
