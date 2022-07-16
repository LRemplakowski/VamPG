using SunsetSystems.Loading;
using System;
using UnityEngine;


public static class StateManager
{
    [SerializeField]
    private static GameState currentState = GameState.Menu;

    public static event Action<GameState, GameState> OnGameStateChanged;

    static StateManager()
    {
        LoadingScreenController.LoadingScreenEnabled += SetStatePaused;
        LoadingScreenController.LoadingScreenDisabled += SetStateExploration;
    }

    private static void SetStatePaused()
    {
        SetCurrentState(GameState.GamePaused);
    }

    private static void SetStateExploration()
    {
        SetCurrentState(GameState.Exploration);
    }

    public static GameState GetCurrentState()
    {
        return currentState;
    }

    public static void SetCurrentState(GameState newState)
    {
        OnGameStateChanged?.Invoke(newState, currentState);
        Debug.Log("Setting new state: " + newState.ToString());
        currentState = newState;
    }

    public static void SetCurrentState(int stateID)
    {
        SetCurrentState((GameState)stateID);
    }
}
