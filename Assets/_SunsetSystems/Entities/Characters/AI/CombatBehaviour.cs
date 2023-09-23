using System;
using System.Collections.Generic;
using UnityEngine;
using Apex.AI;
using Apex.AI.Components;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Game;
using SunsetSystems.Combat;
using SunsetSystems.Resources;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Inventory;
using SunsetSystems.Entities;
using SunsetSystems.Spellbook;
using Sirenix.OdinInspector;
using System.Threading.Tasks;

public class CombatBehaviour : MonoBehaviour, IContextProvider, ICombatant
{
    private CreatureContext _context;
    private CreatureContext Context
    {
        get
        {
            if (_context == null)
                _context = new(this, CombatManager.Instance);
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
    }

    #region Enable&Disable
    private void OnEnable()
    {
        HostileAction.OnAttackFinished += OnHostileActionFinished;
        CombatManager.CombatRoundBegin += OnCombatRoundBegin;
        CombatManager.CombatRoundEnd += OnCombatRoundEnd;
    }

    private void OnDisable()
    {
        HostileAction.OnAttackFinished -= OnHostileActionFinished;
        CombatManager.CombatRoundBegin -= OnCombatRoundBegin;
        CombatManager.CombatRoundEnd -= OnCombatRoundEnd;
    }
    #endregion


    private void Start()
    {
        if (!Owner)
            Owner = GetComponentInParent<Creature>();
        enabled = false;
    }

    private void Update()
    {
        if (GameManager.IsCurrentState(GameState.Combat) && Owner != null && CombatManager.CurrentActiveActor != null)
            if (!IsPlayerControlled && Owner.Equals(CombatManager.CurrentActiveActor))
                if (HasMoved && HasActed)
                    CombatManager.Instance.NextRound();
    }

    private void OnHostileActionFinished(ICombatant target, ICombatant performer)
    {
        if (performer.Equals(this))
        {
            HasActed = true;
        }
    }

    private void OnCombatRoundBegin(ICombatant currentActor)
    {
        if (Owner.Equals(currentActor))
        {
            HasMoved = false;
            HasActed = false;

            if (IsPlayerControlled)
            {
                CachedMultiLevelGrid grid = CombatManager.Instance.CurrentEncounter.MyGrid;
                grid.HighlightCellsInRange(grid.WorldPositionToGridPosition(Owner.transform.position), Owner.MovementRange, Owner.References.NavMeshAgent);
            }
        }
    }

    private void OnCombatRoundEnd(ICombatant currentActor)
    {
        if (Owner.Equals(currentActor) && IsPlayerControlled)
        {
            CombatManager.Instance.CurrentEncounter.MyGrid.RestoreHighlightedCellsToPreviousState();
        }
    }

    public IAIContext GetContext(Guid aiId)
    {
        return Context;
    }

    #region ICombatant
    [field: SerializeField]
    public IMagicUser MagicUser { get; private set; }
    public IWeapon CurrentWeapon => throw new NotImplementedException();

    public IWeapon PrimaryWeapon => throw new NotImplementedException();

    public IWeapon SecondaryWeapon => throw new NotImplementedException();

    public Vector3 AimingOrigin => throw new NotImplementedException();

    public bool IsInCover => throw new NotImplementedException();

    public IList<ICover> CurrentCoverSources => throw new NotImplementedException();

    public int MovementRange => throw new NotImplementedException();
    [field: ShowInInspector, ReadOnly]
    public bool HasActed { get; private set; }
    [field: ShowInInspector, ReadOnly]
    public bool HasMoved { get; private set; }

    public string ID => Owner.ID;

    public string Name => Owner.Name;

    public Faction Faction => Owner.Faction;

    public IEntityReferences References => Owner.References;

    public Transform Transform => Owner.Transform;

    public bool TakeDamage(int amount)
    {
        throw new NotImplementedException();
    }

    public int GetAttributeValue(AttributeType attributeType)
    {
        throw new NotImplementedException();
    }

    public Task PerformAction(EntityAction action, bool clearQueue = false) => Owner.PerformAction(action, clearQueue);

    public new T GetComponent<T>() where T : Component => References.GetComponent<T>();
    public new T GetComponentInChildren<T>() where T : Component => References.GetComponentInChildren<T>();

    #endregion
}

