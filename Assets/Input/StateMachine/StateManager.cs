using Systems.Management;
using UnityEngine;
using Utils.Singleton;

public static class StateManager
{
    [SerializeField]
    private static GameState currentState;

    public delegate void GameStateChanged(GameState newState, GameState oldState);
    public static event GameStateChanged OnGameStateChanged;

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
