using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Combat;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Inventory;
using SunsetSystems.Spellbook;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
using System.Linq;

public class CombatBehaviour : SerializedMonoBehaviour, ICombatant
{
    [Title("Config")]
    [SerializeField]
    private float defaultRaycastOriginY = 1.5f;

    [Title("References")]
    [SerializeField]
    private Transform _raycastOrigin;
    public Vector3 RaycastOrigin => _raycastOrigin.position;
    [field: SerializeField]
    public LineRenderer LineRenderer { get; private set; }
    [field: SerializeField]
    public ICreature Owner { get; private set; }
    [field: SerializeField]
    public IMagicUser MagicUser { get; private set; }

    public bool IsPlayerControlled => Owner.Faction is Faction.PlayerControlled;

    #region Enable&Disable
    private void OnEnable()
    {
        HostileAction.OnAttackFinished += OnHostileActionFinished;
        CombatManager.Instance.CombatRoundBegin += OnCombatRoundBegin;
        CombatManager.Instance.CombatRoundEnd += OnCombatRoundEnd;
    }

    private void OnDisable()
    {
        HostileAction.OnAttackFinished -= OnHostileActionFinished;
        CombatManager.Instance.CombatRoundBegin -= OnCombatRoundBegin;
        CombatManager.Instance.CombatRoundEnd -= OnCombatRoundEnd;
    }
    #endregion

    private void OnHostileActionFinished(ICombatant target, ICombatant performer)
    {
        if (performer.Equals(this))
        {
            HasActed = true;
        }
    }

    private void OnCombatRoundBegin(ICombatant currentActor)
    {
        if (currentActor.Equals(this))
        {
            HasMoved = false;
            HasActed = false;

            if (IsPlayerControlled)
            {
                CachedMultiLevelGrid grid = CombatManager.Instance.CurrentEncounter.MyGrid;
                grid.HighlightCellsInRange(grid.WorldPositionToGridPosition(Owner.References.Transform.position), MovementRange, Owner.References.NavMeshAgent);
            }
        }
    }

    private void OnCombatRoundEnd(ICombatant currentActor)
    {
        if (currentActor.Equals(this) && IsPlayerControlled)
        {
            CombatManager.Instance.CurrentEncounter.MyGrid.RestoreHighlightedCellsToPreviousState();
        }
    }

    #region ICombatant
    public IWeapon CurrentWeapon => Owner.References.WeaponManager.GetSelectedWeapon();

    public IWeapon PrimaryWeapon => Owner.References.WeaponManager.GetPrimaryWeapon();

    public IWeapon SecondaryWeapon => Owner.References.WeaponManager.GetSecondaryWeapon();

    public Vector3 AimingOrigin => RaycastOrigin;

    public bool IsInCover => CurrentCoverSources.Count > 0;

    public IList<ICover> CurrentCoverSources => new List<ICover>();

    public int MovementRange => Owner.References.StatsManager.GetCombatSpeed();
    [field: Title("Combat Runtime")]
    [field: ShowInInspector, ReadOnly]
    public bool HasActed { get; private set; }
    [field: ShowInInspector, ReadOnly]
    public bool HasMoved { get; private set; }

    public string ID => Owner.ID;

    public string Name => Owner.Name;

    public Faction Faction => Owner.Faction;

    public IEntityReferences References => Owner.References;

    public EntityAction PeekCurrentAction => Owner.PeekCurrentAction;

    public Transform Transform => Owner.Transform;

    public bool TakeDamage(int amount)
    {
        Owner.References.StatsManager.TakeDamage(amount);
        return true;
    }

    public int GetAttributeValue(AttributeType attributeType)
    {
        return Owner.References.StatsManager.GetAttributes().FirstOrDefault(attribute => attribute.AttributeType == attributeType).GetValue();
    }

    public Task PerformAction(EntityAction action, bool clearQueue = false) => Owner.PerformAction(action, clearQueue);

    public new T GetComponent<T>() where T : Component => References.GetComponent<T>();
    public new T GetComponentInChildren<T>() where T : Component => References.GetComponentInChildren<T>();

    public void SignalEndTurn()
    {
        CombatManager.Instance.NextRound();
    }

    #endregion
}

