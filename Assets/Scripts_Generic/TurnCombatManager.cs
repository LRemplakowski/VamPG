using Entities.Characters;
using SunsetSystems.Game;
using SunsetSystems.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCombatManager : Singleton<TurnCombatManager>
{
    public delegate void OnCombatStart(List<Creature> creaturesInCombat);
    public static event OnCombatStart NotifyCombatStart;
    public delegate void OnCombatEnd();
    public static event OnCombatEnd NotifyCombatEnd;
    public delegate void OnActiveActorChanged(Creature newActor, Creature previousActor);
    public static event OnActiveActorChanged NotifyActiveActorChanged;
    public delegate void OnCombatRoundBegin(Creature currentActor);
    public static event OnCombatRoundBegin NotifyCombatRoundBegin;
    public delegate void OnCombatRoundEnd(Creature currentActor);
    public static event OnCombatRoundEnd NotifyCombatRoundEnd;

    private int roundCounter;

    [SerializeField, ReadOnly]
    private Creature _currentActiveActor;
    public Creature CurrentActiveActor
    {
        get => _currentActiveActor;
        set
        {
            Creature previous = _currentActiveActor;
            _currentActiveActor = value;
            if (NotifyActiveActorChanged != null)
                NotifyActiveActorChanged.Invoke(value, previous);
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
        if (GameManager.Instance.IsCurrentState(GameState.Combat))
        {
            foreach (Creature c in creaturesInCombat)
            {
                if (c.GetComponent<StatsManager>().IsAlive() && c.Data.Faction.Equals(Faction.Hostile))
                {
                    break;
                }
                else
                {
                    NotifyCombatEnd?.Invoke();
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
        if (previousActor != null && NotifyCombatRoundEnd != null)
        {
            NotifyCombatRoundEnd.Invoke(previousActor);
        }
        if (newActor != null && NotifyCombatRoundBegin != null)
        {
            NotifyCombatRoundBegin.Invoke(newActor);
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
            if (NotifyCombatRoundEnd != null && CurrentActiveActor != null)
                NotifyCombatRoundEnd.Invoke(CurrentActiveActor);
            _currentActiveActor = null;
            if (NotifyCombatEnd != null)
                NotifyCombatEnd.Invoke();
        }
    }

    private IEnumerator InitializeCombat()
    {
        roundCounter = 0;
        _gridInstance = GameManager.Instance.GetGridController();
        creaturesInCombat = FindObjectsOfType<Creature>();
        if (NotifyCombatStart != null)
        {
            NotifyCombatStart.Invoke(new List<Creature>(creaturesInCombat));
        }
        yield return new WaitUntil(() => AllCreaturesMoved());
        roundCounter = 1;
        CurrentActiveActor = GameManager.Instance.GetMainCharacter();
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
