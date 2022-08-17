using System;
using System.Collections.Generic;
using UnityEngine;
using Apex.AI;
using Apex.AI.Components;
using Entities.Characters;
using Entities.Characters.Actions;
using SunsetSystems.Game;
using SunsetSystems.Combat;

public class CombatBehaviour : ExposableMonobehaviour, IContextProvider
{
    private CreatureContext _context;
    private CombatManager cachedCombatManager;

    [SerializeField]
    private Transform _raycastOrigin;
    public Vector3 RaycastOrigin => _raycastOrigin.position;
    [SerializeField]
    private float defaultRaycastOriginY = 1.5f;

    public Creature Owner { get; private set; }

    public bool HasActed { get; set; }

    public bool HasMoved { get; set; }

    public bool IsPlayerControlled => _context.IsPlayerControlled;

    private void Reset()
    {
        if (!_raycastOrigin)
        {
            _raycastOrigin = new GameObject("RaycastOrigin").transform;
            _raycastOrigin.parent = this.transform;
            _raycastOrigin.localPosition = new Vector3(0, defaultRaycastOriginY, 0);
        }
    }

    private void Awake()
    {
        if (!_raycastOrigin)
        {
            _raycastOrigin = new GameObject("RaycastOrigin").transform;
            _raycastOrigin.parent = this.transform;
            _raycastOrigin.localPosition = new Vector3(0, defaultRaycastOriginY, 0);
        }
        Owner = GetComponent<Creature>();
    }

    #region Enable&Disable
    private void OnEnable()
    {
        HostileAction.onAttackFinished += OnHostileActionFinished;
        CombatManager.CombatBegin += OnCombatStart;
        CombatManager.CombatRoundBegin += OnCombatRoundBegin;
        CombatManager.CombatRoundEnd += OnCombatRoundEnd;
        CombatManager.CombatEnd += OnCombatEnd;
    }

    private void OnDisable()
    {
        HostileAction.onAttackFinished -= OnHostileActionFinished;
        CombatManager.CombatBegin -= OnCombatStart;
        CombatManager.CombatRoundBegin -= OnCombatRoundBegin;
        CombatManager.CombatRoundEnd -= OnCombatRoundEnd;
        CombatManager.CombatEnd -= OnCombatEnd;
    }
    #endregion

    private void Start()
    {
        if (!cachedCombatManager)
            cachedCombatManager = this.FindFirstComponentWithTag<CombatManager>(TagConstants.COMBAT_MANAGER);
        _context = new CreatureContext(Owner, cachedCombatManager);
    }

    private void Update()
    {
        if (GameManager.IsCurrentState(GameState.Combat))
            if (!IsPlayerControlled && cachedCombatManager.CurrentActiveActor.Equals(this.Owner))
                if (HasMoved && HasActed)
                    cachedCombatManager.NextRound();
    }

    private void OnMovementStarted(Creature who)
    {
        if (who.Equals(Owner) && IsPlayerControlled)
        {
            cachedCombatManager.CurrentEncounter.MyGrid.ClearActiveElements();
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
                cachedCombatManager.CurrentEncounter.MyGrid.ActivateElementsInRangeOfActor(Owner);
            }
        }
    }

    private void OnCombatRoundEnd(Creature currentActor)
    {
        if (currentActor.Equals(Owner) && IsPlayerControlled)
        {
            cachedCombatManager.CurrentEncounter.MyGrid.ClearActiveElements();
        }
    }

    public IAIContext GetContext(Guid aiId)
    {
        return _context;
    }
}

