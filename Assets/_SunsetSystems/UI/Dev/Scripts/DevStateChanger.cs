using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Game
{
    [RequireComponent(typeof(Dropdown))]
    public class DevStateChanger : MonoBehaviour
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
            dropdown.SetValueWithoutNotify((int)GameManager.CurrentState);
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
            GameManager.CurrentState = (GameState)dropdown.value;
        }
    }
}
