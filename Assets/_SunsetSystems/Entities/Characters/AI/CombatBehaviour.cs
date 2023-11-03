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
using SunsetSystems.AI;
using UltEvents;

namespace SunsetSystems.Combat
{
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

        private EntityAction currentAction;

        #region Unity Messages
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

        private void Update()
        {
            if (CombatManager.Instance.CurrentActiveActor?.Equals(this) ?? false)
            {
                if (currentAction != null && currentAction.ActionFinished)
                {
                    currentAction = null;
                }
                if (currentAction == null && HasMoved && HasActed)
                {
                    SignalEndTurn();
                }
            }
        }
        #endregion

        private void OnHostileActionFinished(ICombatant target, ICombatant performer)
        {
            if (performer.Equals(this))
            {
                HasActed = true;
            }
        }

        [Button]
        private void OnCombatRoundBegin(ICombatant currentActor)
        {
            if (currentActor.Equals(this))
            {
                HasMoved = false;
                HasActed = false;

                if (IsPlayerControlled)
                {
                    GridManager grid = CombatManager.Instance.CurrentEncounter.GridManager;
                    grid.ShowCellsInMovementRange(grid.WorldPositionToGridPosition(Owner.References.Transform.position), this);
                }
            }
        }

        [Button]
        private void OnCombatRoundEnd(ICombatant currentActor)
        {
            if (currentActor.Equals(this) && IsPlayerControlled)
            {
                CombatManager.Instance.CurrentEncounter.GridManager.HideCellsInMovementRange();
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
        public int SprintRange => MovementRange * 2;
        [field: Title("Events")]
        [field: SerializeField]
        public UltEvent OnChangedGridPosition { get; set; }

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
            return Owner.References.StatsManager.GetAttributes().FirstOrDefault(attribute => attribute.AttributeType == attributeType)?.GetValue() ?? 1;
        }

        public Task PerformAction(EntityAction action, bool clearQueue = false)
        {
            currentAction = action;
            Debug.Log($"Beginning action {action}");
            if (action is Move)
                HasMoved = true;
            else
                HasActed = true;
            return Owner.PerformAction(action, clearQueue);
        }

        public new T GetComponent<T>() where T : Component => References.GetComponent<T>();
        public new T GetComponentInChildren<T>() where T : Component => References.GetComponentInChildren<T>();

        public void SignalEndTurn()
        {
            if (CombatManager.Instance.CurrentActiveActor?.Equals(this) ?? false)
                CombatManager.Instance.NextRound();
        }

        public bool MoveToGridPosition(Vector3Int gridPosition)
        {
            GridManager gridManager = CombatManager.Instance.CurrentEncounter.GridManager;
            GridUnit gridUnit = gridManager[gridPosition];
            return true;
        }

        public bool AttackCreatureUsingCurrentWeapon(ICombatant target)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        public override string ToString()
        {
            return $"{base.ToString()} - {Owner}";
        }
    }
}


