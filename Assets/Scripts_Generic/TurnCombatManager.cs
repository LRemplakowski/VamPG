using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnCombatManager : ExposableMonobehaviour
{
    public delegate void OnCombatStart();
    public OnCombatStart onCombatStart;
    public delegate void OnCombatEnd();
    public OnCombatEnd onCombatEnd;
    public delegate void OnActiveActorChanged(Creature newActor, Creature previousActor);
    public OnActiveActorChanged onActiveActorChanged;

    #region Instance
    public static TurnCombatManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    #region Enable&Disable
    private void OnEnable()
    {
        StateManager.instance.onGameStateChanged += MaybeStartOrEndCombat;
    }
    private void OnDisable()
    {
        StateManager.instance.onGameStateChanged -= MaybeStartOrEndCombat;
    }
    #endregion

    [SerializeField, ReadOnly]
    private Creature _currentActiveActor;
    public Creature CurrentActiveActor 
    {
        get => _currentActiveActor; 
        set
        {
            if (onActiveActorChanged != null)
                onActiveActorChanged.Invoke(value, _currentActiveActor);
            _currentActiveActor = value;
        }
    }
    [SerializeField]
    private GridController _gridInstance;
    public GridController GridInstance 
    {
        get => _gridInstance;
        set => _gridInstance = value; 
    }

    private Creature[] creaturesInCombat;

    public List<Creature> GetCreaturesInCombat()
    {
        return new List<Creature>(creaturesInCombat);
    }

    public void SetCurrentActiveActor(int index)
    {
        Creature c = null;
        if (index < creaturesInCombat.Length)
            c = creaturesInCombat[index];
        CurrentActiveActor = c;
    }

    private void MaybeStartOrEndCombat(GameState newState, GameState oldState)
    {
        if (newState == GameState.Combat)
        {
            creaturesInCombat = FindObjectsOfType<Creature>();
            CurrentActiveActor = GameManager.GetPlayer();
            if (onCombatStart != null)
            {
                onCombatStart.Invoke();
                Debug.Log("onCombatStart called");
            }
        }
        else if (oldState == GameState.Combat)
        {
            if (onCombatEnd != null)
                onCombatEnd.Invoke();
        }
    }

    public bool IsActiveActorPlayer()
    {
        return _currentActiveActor.IsOfType(typeof(Player));
    }

    public bool IsActiveActorNPC()
    {
        return _currentActiveActor.IsOfType(typeof(NPC));
    }
}
