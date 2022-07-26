using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SunsetSystems.Game.StateManager;

namespace SunsetSystems.Game
{
    [RequireComponent(typeof(Dropdown))]
    public class DevStateChanger : ExposableMonobehaviour
    {
        private Dropdown dropdown;

        private void OnEnable()
        {
            dropdown = GetComponent<Dropdown>();
            dropdown.ClearOptions();
            List<Dropdown.OptionData> options = new();
            foreach (int g in Enum.GetValues(typeof(GameState)))
            {
                options.Add(new Dropdown.OptionData(((GameState)g).ToString()));
            }
            dropdown.AddOptions(options);
            dropdown.SetValueWithoutNotify((int)GameManager.Instance.GetCurrentState());
            dropdown.onValueChanged.AddListener(delegate { ChangeState(); });
        }

        private void OnDisable()
        {
            dropdown.onValueChanged.RemoveListener(delegate { ChangeState(); });
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

        private void ChangeState()
        {
            GameManager.Instance.OverrideState((GameState)dropdown.value);
        }
    }
}
