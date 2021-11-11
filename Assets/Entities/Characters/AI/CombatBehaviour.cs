using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apex.AI;
using Apex.AI.Components;

[RequireComponent(typeof(StatsManager))]
public class CombatBehaviour : ExposableMonobehaviour, IContextProvider
{
    private CreatureContext _context;

    [SerializeField]
    private Transform _raycastOrigin;
    public Vector3 RaycastOrigin => _raycastOrigin.position;
    [SerializeField]
    private float defaultRaycastOriginY = 1.5f;

    public Creature Owner => GetComponent<Creature>();

    public bool HasActed { get; set; }

    public bool HasMoved { get; set; }

    public bool IsPlayerControlled => _context.IsPlayerControlled;


    private void Awake()
    {
        _context = new CreatureContext(Owner);
    }

    #region Enable&Disable
    private void OnEnable()
    {
        if (!_raycastOrigin)
        {
            _raycastOrigin = new GameObject("RaycastOrigin").transform;
            _raycastOrigin.parent = this.transform;
            _raycastOrigin.localPosition = new Vector3(0, defaultRaycastOriginY, 0);
        }
        HostileAction.onAttackFinished += OnHostileActionFinished;
        TurnCombatManager.onCombatStart += OnCombatStart;
        TurnCombatManager.onCombatRoundBegin += OnCombatRoundBegin;
        TurnCombatManager.onCombatRoundEnd += OnCombatRoundEnd;
        TurnCombatManager.onCombatEnd += OnCombatEnd;
    }

    private void OnDisable()
    {
        HostileAction.onAttackFinished -= OnHostileActionFinished;
        TurnCombatManager.onCombatStart -= OnCombatStart;
        TurnCombatManager.onCombatRoundBegin -= OnCombatRoundBegin;
        TurnCombatManager.onCombatRoundEnd -= OnCombatRoundEnd;
        TurnCombatManager.onCombatEnd -= OnCombatEnd;
    }
    #endregion

    private void OnMovementStarted(Creature who)
    {
        if (who.Equals(Owner) && IsPlayerControlled)
        {
            GameManager.GetGridController().ClearActiveElements();
        }
    }

    private void OnMovementFinished(Creature who)
    {
        if (who.Equals(Owner))
        {
            HasMoved = true;
        }
    }

    private void OnHostileActionFinished(Creature target, Creature performer)
    {
        if (performer.Equals(Owner))
        {
            HasActed = true;
        }
    }

    private void OnCombatStart(List<Creature> creaturesInCombat)
    {
        Move.onMovementStarted += OnMovementStarted;
        Move.onMovementFinished += OnMovementFinished;
        HasMoved = false;
        HasActed = false;
        GridElement nearest = GameManager.GetGridController().GetNearestGridElement(this.transform.position);
        Owner.Move(nearest);
    }

    private void OnCombatEnd()
    {
        Move.onMovementStarted -= OnMovementStarted;
        Move.onMovementFinished -= OnMovementFinished;
    }

    private void OnCombatRoundBegin(Creature currentActor)
    {
        if (currentActor.Equals(Owner))
        {
            HasMoved = false;
            HasActed = false;

            if (IsPlayerControlled)
            {
                GameManager.GetGridController().ActivateElementsInRangeOfActor(Owner);
            }
        }
    }
    
    private void OnCombatRoundEnd(Creature currentActor)
    {
        if (currentActor.Equals(Owner) && IsPlayerControlled)
        {
            GameManager.GetGridController().ClearActiveElements();
        }
    }

    public IAIContext GetContext(Guid aiId)
    {
        return _context;
    }
}

