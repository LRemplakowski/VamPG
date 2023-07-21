using SunsetSystems.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevGUIManager : MonoBehaviour
{
    private DevTurnCombatGUI devTurnCombatGUI;

    private void Start()
    {
        devTurnCombatGUI = GetComponentInChildren<DevTurnCombatGUI>();
        devTurnCombatGUI.gameObject.SetActive(GameManager.IsCurrentState(GameState.Combat));
    }
}
