using System;
using System.Collections.Generic;
using UnityEngine;
using Apex.AI;
using Apex.AI.Components;
using Entities.Characters;
using Entities.Characters.Actions;
using SunsetSystems.Game;
using SunsetSystems.Combat;
using SunsetSystems.Resources;

public class CombatBehaviour : ExposableMonobehaviour, IContextProvider
{
    private CreatureContext _context;
    private CreatureContext Context
    {
        get
        {
            if (_context == null)
                _context = new(this.Owner, CombatManager.Instance);
            return _context;
        }
    }

    [SerializeField]
    private Transform _raycastOrigin;
    public Vector3 RaycastOrigin => _raycastOrigin.position;
    [SerializeField]
    private float defaultRaycastOriginY = 1.5f;
    [field: SerializeField]
    public LineRenderer LineRenderer { get; private set; }

    public Creature Owner { get; private set; }

    public bool HasActed { get; set; }

    public bool HasMoved { get; set; }

    public bool IsPlayerControlled => Context.IsPlayerControlled;

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
        if (!LineRenderer)
        {
            LineRenderer = Instantiate(ResourceLoader.GetTargetingLineRendererPrefab(), RaycastOrigin, Quaternion.identity, transform);
            LineRenderer.enabled = false;
        }
        CombatManager.CombatBegin += OnCombatStart;
        CombatManager.CombatEnd += OnCombatEnd;
    }

    #region Enable&Disable
    private void OnEnable()
    {
        HostileAction.onAttackFinished += OnHostileActionFinished;
        CombatManager.CombatRoundBegin += OnCombatRoundBegin;
        CombatManager.CombatRoundEnd += OnCombatRoundEnd;
    }

    private void OnDisable()
    {
        HostileAction.onAttackFinished -= OnHostileActionFinished;
        CombatManager.CombatRoundBegin -= OnCombatRoundBegin;
        CombatManager.CombatRoundEnd -= OnCombatRoundEnd;
    }
    #endregion

    private void OnDestroy()
    {
        CombatManager.CombatBegin -= OnCombatStart;
        CombatManager.CombatEnd -= OnCombatEnd;
    }

    private void Start()
    {
        if (!Owner)
            Owner = GetComponent<Creature>();
        enabled = false;
    }

    private void Update()
    {
        if (GameManager.IsCurrentState(GameState.Combat))
            if (!IsPlayerControlled && Owner.Equals(CombatManager.CurrentActiveActor))
                if (HasMoved && HasActed)
                    CombatManager.Instance.NextRound();
    }

    private void OnMovementStarted(Creature who)
    {
        if (who.Equals(Owner) && IsPlayerControlled)
        {
            CombatManager.Instance.CurrentEncounter.MyGrid.ClearActiveElements();
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
        if (creaturesInCombat.Contains(Owner))
        {
            enabled = true;
            Move.onMovementStarted += OnMovementStarted;
            Move.onMovementFinished += OnMovementFinished;
            HasMoved = false;
            HasActed = false;
        }
    }

    private void OnCombatEnd()
    {
        enabled = false;
        Move.onMovementStarted -= OnMovementStarted;
        Move.onMovementFinished -= OnMovementFinished;
    }

    private void OnCombatRoundBegin(Creature currentActor)
    {
        if (Owner.Equals(currentActor))
        {
            HasMoved = false;
            HasActed = false;

            if (IsPlayerControlled)
            {
                CombatManager.Instance.CurrentEncounter.MyGrid.ActivateElementsInRangeOfActor(Owner);
            }
        }
    }

    private void OnCombatRoundEnd(Creature currentActor)
    {
        if (Owner.Equals(currentActor) && IsPlayerControlled)
        {
            CombatManager.Instance.CurrentEncounter.MyGrid.ClearActiveElements();
        }
    }

    public IAIContext GetContext(Guid aiId)
    {
        return Context;
    }
}

