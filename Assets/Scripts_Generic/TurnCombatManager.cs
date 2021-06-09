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
    public delegate void OnCombatRoundBegin(Creature currentActor);
    public OnCombatRoundBegin onCombatRoundBegin;
    public delegate void OnCombatRoundEnd(Creature currentActor);
    public OnCombatRoundEnd onCombatRoundEnd;

    private int roundCounter;

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
        onActiveActorChanged += NewRound;
    }
    private void OnDisable()
    {
        StateManager.instance.onGameStateChanged -= MaybeStartOrEndCombat;
        onActiveActorChanged -= NewRound;
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

    private void NewRound(Creature newActor, Creature previousActor)
    {
        if (previousActor != null && onCombatRoundEnd != null)
        {
            onCombatRoundEnd.Invoke(previousActor);
        }
        if (newActor != null && onCombatRoundBegin != null)
        {
            onCombatRoundBegin.Invoke(newActor);
        }
    }

    public void NextRound()
    {
        int index = GetCreaturesInCombat().IndexOf(CurrentActiveActor);
        SetCurrentActiveActor(++index < creaturesInCombat.Length ? index : 0);
    }

    private void MaybeStartOrEndCombat(GameState newState, GameState oldState)
    {
        if (newState == GameState.Combat)
        {
            roundCounter = 0;
            _gridInstance = GameManager.GetGridController();
            creaturesInCombat = FindObjectsOfType<Creature>();
            if (onCombatStart != null)
            {
                onCombatStart.Invoke();
            }
            roundCounter = 1;
            CurrentActiveActor = GameManager.GetPlayer();
        }
        else if (oldState == GameState.Combat)
        {
            roundCounter = 0;
            if (onCombatRoundEnd != null && _currentActiveActor != null)
                onCombatRoundEnd.Invoke(_currentActiveActor);
            _currentActiveActor = null;
            if (onCombatEnd != null)
                onCombatEnd.Invoke();
        }
    }

    public bool IsBeforeFirstRound()
    {
        return roundCounter <= 0;
    }

    public bool IsFirstRoundOfCombat()
    {
        return roundCounter == 1;
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
