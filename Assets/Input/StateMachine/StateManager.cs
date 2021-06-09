using UnityEngine;

[System.Serializable]
public class StateManager : ExposableMonobehaviour
{
    #region Instance
    public static StateManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    [SerializeField]
    private GameState currentState;

    public delegate void OnGameStateChanged(GameState newState, GameState oldState);
    public OnGameStateChanged onGameStateChanged;

    public GameState GetCurrentState()
    {
        return currentState;
    }

    public void SetCurrentState(GameState newState)
    {
        if (onGameStateChanged != null)
        {
            onGameStateChanged.Invoke(newState, currentState);
        }
        currentState = newState;
    }

    public void SetCurrentState(int stateID)
    {
        GameState newState = (GameState)stateID;
        SetCurrentState(newState);
    }
}
