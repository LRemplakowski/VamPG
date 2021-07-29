using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnCombatManager : ExposableMonobehaviour
{
    public delegate void OnCombatStart(List<Creature> creaturesInCombat);
    public static OnCombatStart onCombatStart;
    public delegate void OnCombatEnd();
    public static OnCombatEnd onCombatEnd;
    public delegate void OnActiveActorChanged(Creature newActor, Creature previousActor);
    public static OnActiveActorChanged onActiveActorChanged;
    public delegate void OnCombatRoundBegin(Creature currentActor);
    public static OnCombatRoundBegin onCombatRoundBegin;
    public delegate void OnCombatRoundEnd(Creature currentActor);
    public static OnCombatRoundEnd onCombatRoundEnd;

    private int roundCounter;

    #region Instance
    public static TurnCombatManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Enable&Disable
    private void OnEnable()
    {
        StateManager.instance.onGameStateChanged -= MaybeStartOrEndCombat;
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
            Creature previous = _currentActiveActor;
            _currentActiveActor = value;
            if (onActiveActorChanged != null)
                onActiveActorChanged.Invoke(value, previous);
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

    private void Update()
    {
        if (StateManager.instance.GetCurrentState().Equals(GameState.Combat))
        {
            foreach (Creature c in creaturesInCombat)
            {
                if (c.GetComponent<StatsManager>().IsAlive() && c.Faction.Equals(Faction.Hostile))
                {
                    break;
                }
                else
                {
                    StateManager.instance.SetCurrentState(GameState.Exploration);
                }
            }
        }
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
        roundCounter++;
        int index = GetCreaturesInCombat().IndexOf(CurrentActiveActor);
        SetCurrentActiveActor(++index < creaturesInCombat.Length ? index : 0);
    }

    private void MaybeStartOrEndCombat(GameState newState, GameState oldState)
    {
        if (newState == GameState.Combat)
        {
            StartCoroutine(InitializeCombat());
        }
        else if (oldState == GameState.Combat)
        {
            roundCounter = 0;
            if (onCombatRoundEnd != null && CurrentActiveActor != null)
                onCombatRoundEnd.Invoke(CurrentActiveActor);
            _currentActiveActor = null;
            if (onCombatEnd != null)
                onCombatEnd.Invoke();
        }
    }

    private IEnumerator InitializeCombat()
    {
        roundCounter = 0;
        _gridInstance = GameManager.GetGridController();
        creaturesInCombat = FindObjectsOfType<Creature>();
        if (onCombatStart != null)
        {
            onCombatStart.Invoke(new List<Creature>(creaturesInCombat));
        }
        yield return new WaitUntil(() => AllCreaturesMoved());
        roundCounter = 1;
        CurrentActiveActor = GameManager.GetPlayer();
        StopCoroutine(InitializeCombat());
    }

    private bool AllCreaturesMoved()
    {
        foreach (Creature c in creaturesInCombat)
        {
            if (!c.GetComponent<CombatBehaviour>().HasMoved)
                return false; 
        }
        return true;
    }

    public bool IsBeforeFirstRound()
    {
        return roundCounter <= 0;
    }

    public bool IsFirstRoundOfCombat()
    {
        return roundCounter == 1;
    }

    public bool IsActiveActorPlayerControlled()
    {
        return _currentActiveActor ? CurrentActiveActor.GetComponent<CombatBehaviour>().IsPlayerControlled : false;
    }

    public int GetRound()
    {
        return roundCounter;
    }
}
