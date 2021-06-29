using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevGUIManager : ExposableMonobehaviour
{
    #region Enable&Disable
    private void OnEnable()
    {
        StateManager.instance.onGameStateChanged += MaybeShowOrHideDevTurnCombatGUI;
    }

    private void OnDisable()
    {
        StateManager.instance.onGameStateChanged -= MaybeShowOrHideDevTurnCombatGUI;
    }
    #endregion

    private DevTurnCombatGUI devTurnCombatGUI;

    private void Start()
    {
        devTurnCombatGUI = GetComponentInChildren<DevTurnCombatGUI>();
        devTurnCombatGUI.gameObject.SetActive(StateManager.instance.GetCurrentState() == GameState.Combat);
    }

    public void MaybeShowOrHideDevTurnCombatGUI(GameState newState, GameState oldState)
    {
        devTurnCombatGUI.gameObject.SetActive(newState == GameState.Combat);
    }
}
