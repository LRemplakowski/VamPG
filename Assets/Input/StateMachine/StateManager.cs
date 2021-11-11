using UnityEngine;
using Utils.Singleton;

[System.Serializable]
public class StateManager : Singleton<StateManager>
{

    [SerializeField]
    private GameState currentState;

    public delegate void GameStateChanged(GameState newState, GameState oldState);
    public static event GameStateChanged OnGameStateChanged;

    public GameState GetCurrentState()
    {
        return currentState;
    }

    public void SetCurrentState(GameState newState)
    {
        currentState = newState;
        if (OnGameStateChanged != null)
        {
            OnGameStateChanged.Invoke(newState, currentState);
        }
    }

    public void SetCurrentState(int stateID)
    {
        GameState newState = (GameState)stateID;
        SetCurrentState(newState);
    }
}
