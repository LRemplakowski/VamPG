using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class DevStateChanger : ExposableMonobehaviour
{
    private void Start()
    {
        Dropdown dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (int g in Enum.GetValues(typeof(GameState)))
        {
            options.Add(new Dropdown.OptionData(((GameState)g).ToString()));
        }
        dropdown.AddOptions(options);
        dropdown.SetValueWithoutNotify((int)StateManager.instance.GetCurrentState());
    }
}
