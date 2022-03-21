using SunsetSystems.Management;
using System;
using UnityEngine;


public static class StateManager
{
    [SerializeField]
    private static GameState currentState = GameState.MainMenu;

    public static event Action<GameState, GameState> OnGameStateChanged;

    public static GameState GetCurrentState()
    {
        return currentState;
    }

    public static void SetCurrentState(GameState newState)
    {
        currentState = newState;
        if (OnGameStateChanged != null)
        {
            OnGameStateChanged.Invoke(newState, currentState);
        }
    }

    public static void SetCurrentState(int stateID)
    {
        GameState newState = (GameState)stateID;
        SetCurrentState(newState);
    }
}
