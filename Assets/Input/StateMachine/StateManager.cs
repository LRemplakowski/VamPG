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

    public GameState GetCurrentState()
    {
        return currentState;
    }
}
