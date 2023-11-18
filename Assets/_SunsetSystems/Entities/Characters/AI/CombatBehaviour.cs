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
using SunsetSystems.Animation;
using SunsetSystems.Equipment;
using Sirenix.Utilities;
using UnityEngine.AI;

namespace SunsetSystems.Combat
{
    public class CombatBehaviour : SerializedMonoBehaviour, ICombatant
    {
        [Title("Config")]
        [SerializeField]
        private float defaultRaycastOriginY = 1.5f;
        [SerializeField]
        private string attackAnimationTrigger;
        private int attackAnimationTriggerHash;
        [SerializeField]
        private string takeHitAnimationTrigger;
        private int takeHitAnimationTriggerHash;
        [SerializeField]
        private string hasCoverAnimationBoolean;
        private int hasCoverAnimationBooleanHash;

        [Title("References")]
        [SerializeField]
        private Transform _raycastOrigin;
        public Vector3 RaycastOrigin => _raycastOrigin.position;
        [SerializeField, Required]
        private IWeaponManager weaponManager;
        [field: SerializeField]
        public LineRenderer LineRenderer { get; private set; }
        [field: SerializeField, Required]
        public ICreature Owner { get; private set; }
        [field: SerializeField, Required]
        public IMagicUser MagicUser { get; private set; }
        [SerializeField, Required]
        private CreatureAnimationController animationController;

        public bool IsPlayerControlled => Owner.Faction is Faction.PlayerControlled;

        #region Unity Messages
        private void OnEnable()
        {
            CombatManager.Instance.CombatBegin += OnCombatBegin;
            CombatManager.Instance.CombatRoundBegin += OnCombatRoundBegin;
            CombatManager.Instance.CombatRoundEnd += OnCombatRoundEnd;
            CombatManager.Instance.CombatEnd += OnCombatEnd;
        }

        private void Start()
        {
            attackAnimationTriggerHash = Animator.StringToHash(attackAnimationTrigger);
            takeHitAnimationTriggerHash = Animator.StringToHash(takeHitAnimationTrigger);
            hasCoverAnimationBooleanHash = Animator.StringToHash(hasCoverAnimationBoolean);
        }

        private void OnDisable()
        {
            CombatManager.Instance.CombatRoundBegin -= OnCombatRoundBegin;
            CombatManager.Instance.CombatRoundEnd -= OnCombatRoundEnd;
        }

        private void Update()
        {
            if (CombatManager.Instance.CurrentActiveActor?.Equals(this) ?? false)
            {
                if (HasActionsQueued is false && HasMoved && HasActed && IsPlayerControlled is false)
                {
                    SignalEndTurn();
                }
            }
        }
        #endregion

        private void OnCombatBegin(IEnumerable<ICombatant> actorsInCombat)
        {
            weaponManager.SetSelectedWeapon(SelectedWeapon.Primary);
            if (actorsInCombat.Contains(this))
            {
                animationController.SetCombatAnimationsActive(true);
            }
        }

        private void OnCombatEnd()
        {
            animationController.SetCombatAnimationsActive(false);
        }

        private void OnCombatRoundBegin(ICombatant currentActor)
        {
            if (currentActor.Equals(this))
            {
                HasMoved = false;
                HasActed = false;
            }
        }

        private void OnCombatRoundEnd(ICombatant currentActor)
        {

        }

        #region ICombatant
        public IWeapon CurrentWeapon => Owner.References.WeaponManager.GetSelectedWeapon();

        public IWeapon PrimaryWeapon => Owner.References.WeaponManager.GetPrimaryWeapon();

        public IWeapon SecondaryWeapon => Owner.References.WeaponManager.GetSecondaryWeapon();

        public Vector3 AimingOrigin => RaycastOrigin;

        public bool IsInCover => CurrentCoverSources.Count > 0;
        public bool IsAlive => Owner.References.StatsManager.IsAlive();

        public IList<ICover> CurrentCoverSources => new List<ICover>();

        public int MovementRange => Owner.References.StatsManager.GetCombatSpeed();
        public int SprintRange => MovementRange * 2;
        [field: Title("Events")]
        [field: SerializeField]
        public UltEvent<ICombatant> OnChangedGridPosition { get; set; }
        [field: SerializeField]
        public UltEvent<ICombatant> OnUsedActionPoint { get; set; }
        [field: SerializeField]
        public UltEvent<ICombatant> OnSpentBloodPoint { get; set; }

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

        public bool HasActionsQueued => Owner.HasActionsQueued;

        public Transform Transform => Owner.Transform;

        public bool TakeDamage(int amount)
        {
            Owner.References.StatsManager.TakeDamage(amount);
            return true;
        }

        public int GetAttributeValue(AttributeType attributeType)
        {
            return Owner.References.StatsManager.GetAttribute(attributeType)?.GetValue() ?? 0;
        }

        public Task PerformAction(EntityAction action, bool clearQueue = false)
        {
            return Owner.PerformAction(action, clearQueue);
        }

        public new T GetComponent<T>() where T : Component => References.GetComponent<T>();
        public new T GetComponentInChildren<T>() where T : Component => References.GetComponentInChildren<T>();

        public void SignalEndTurn()
        {
            if (CombatManager.Instance.CurrentActiveActor?.Equals(this) ?? false)
                CombatManager.Instance.NextRound();
        }

        [Button]
        public bool MoveToGridPosition(GridUnitObject gridObject)
        {
            return MoveToGridPosition(gridObject.GridPosition);
        }

        public bool MoveToGridPosition(Vector3Int gridPosition)
        {
            if (HasMoved)
                return false;
            GridManager gridManager = CombatManager.Instance.CurrentEncounter.GridManager;
            GridUnit gridUnit = gridManager[gridPosition];
            if (IsPlayerControlled)
            {
                if (gridUnit.IsInMoveRange && !HasMoved)
                {
                    HasMoved = true;
                    OnChangedGridPosition?.Invoke(this);
                    CombatManager.Instance.CurrentEncounter.GridManager.HideCellsInMovementRange();
                    _ = PerformAction(new Move(this, gridUnit, gridManager));
                    CurrentCoverSources.Clear();
                    CurrentCoverSources.AddRange(gridUnit.AdjacentCoverSources);
                    animationController.SetBool(hasCoverAnimationBooleanHash, gridUnit.AdjacentToCover);
                    OnUsedActionPoint?.InvokeSafe(this);
                    return true;
                }
                else if (gridUnit.IsInSprintRange && !HasMoved && !HasActed)
                {
                    HasActed = true;
                    HasMoved = true;
                    OnChangedGridPosition?.Invoke(this);
                    CombatManager.Instance.CurrentEncounter.GridManager.HideCellsInMovementRange();
                    _ = PerformAction(new Move(this, gridUnit, gridManager));
                    CurrentCoverSources.Clear();
                    CurrentCoverSources.AddRange(gridUnit.AdjacentCoverSources);
                    animationController.SetBool(hasCoverAnimationBooleanHash, gridUnit.AdjacentToCover);
                    OnUsedActionPoint?.InvokeSafe(this);
                    return true;
                }
            }
            else
            {
                HasMoved = true;
                OnChangedGridPosition?.Invoke(this);
                _ = PerformAction(new Move(this, gridUnit, gridManager));
                CurrentCoverSources.Clear();
                CurrentCoverSources.AddRange(gridUnit.AdjacentCoverSources);
                animationController.SetBool(hasCoverAnimationBooleanHash, gridUnit.AdjacentToCover);
                OnUsedActionPoint?.InvokeSafe(this);
                return true;
            }
            return false;
        }

        [Button]
        public bool AttackCreatureUsingCurrentWeapon(ICombatant target)
        {
            if (HasActed)
                return false;
            IWeapon currentWeapon = weaponManager.GetSelectedWeapon();
            if (currentWeapon == null || currentWeapon.WeaponType is WeaponType.Melee)
            {
                GridManager gridManager = CombatManager.Instance.CurrentEncounter.GridManager;
                Vector3Int currentGridPosition = gridManager.WorldPositionToGridPosition(Transform.position);
                Vector3Int targetGridPosition = gridManager.WorldPositionToGridPosition(target.Transform.position);
                if (Vector3Int.Distance(targetGridPosition, currentGridPosition) < 1.5f)
                {
                    HasActed = true;
                    HasMoved = true;
                    _ = PerformAction(new Attack(target, this));
                    OnUsedActionPoint?.InvokeSafe(this);
                    return true;
                }
                else
                {
                    GridUnit nearestUnitInRangeAdjacentToTarget = FindAdjacentGridPosition(target, gridManager, currentGridPosition, MovementRange, References.GetComponent<NavMeshAgent>());
                    gridManager.ShowCellsInMovementRange(this);
                    if (nearestUnitInRangeAdjacentToTarget != null && MoveToGridPosition(nearestUnitInRangeAdjacentToTarget.GridPosition))
                    {
                        HasMoved = true;
                        HasActed = true;
                        _ = PerformAction(new Attack(target, this));
                        OnUsedActionPoint?.InvokeSafe(this);
                        return true;
                    }
                    gridManager.HideCellsInMovementRange();
                }
            }
            else if (Vector3.Distance(Transform.position, target.Transform.position) <= currentWeapon.GetRangeData().maxRange && weaponManager.UseAmmoFromSelectedWeapon(1))
            {
                HasActed = true;
                HasMoved = true;
                _ = PerformAction(new Attack(target, this));
                OnUsedActionPoint?.InvokeSafe(this);
                return true;
            }
            return false;
        }

        public bool ReloadCurrentWeapon()
        {
            if (HasActed)
                return false;
            weaponManager.ReloadSelectedWeapon();
            return true;
        }

        private static GridUnit FindAdjacentGridPosition(ICombatant target, GridManager grid, Vector3Int currentGridPosition, float movementRange, NavMeshAgent navMeshAgent)
        {
            GridUnit unit = null;
            float distance = float.MaxValue;
            List<GridUnit> positionList = grid.GetCellsInRange(currentGridPosition, movementRange, navMeshAgent, out _);
            if (target != null)
            {
                Vector3Int enemyGridPosition = grid.WorldPositionToGridPosition(target.Transform.position);
                List<GridUnit> walkableCellsNearEnemy = grid.GetCellsInRange(enemyGridPosition, 1.5f, target.References.GetComponent<NavMeshAgent>(), out _);
                IEnumerable<GridUnit> commonElements = positionList.Intersect(walkableCellsNearEnemy);
                foreach (GridUnit commonUnit in commonElements)
                {
                    if (commonUnit != null && commonUnit.IsFree)
                    {
                        float distanceToCommonUnit = Vector3Int.Distance(currentGridPosition, commonUnit.GridPosition);
                        if (distanceToCommonUnit < distance)
                        {
                            unit = commonUnit;
                            distance = distanceToCommonUnit;
                        }
                    }
                }
            }
            return unit;
        }

        /// <summary>
        /// Instructs combatant to perform attack animation with current weapon.
        /// </summary>
        /// <returns>Animation duration</returns>
        public float PerformAttackAnimation()
        {
            animationController.SetTrigger(attackAnimationTriggerHash);
            return 2f;
        }

        /// <summary>
        /// Instructs combatant to perform getting hit animation.
        /// </summary>
        /// <returns>Animation duration</returns>
        public float PerformTakeHitAnimation()
        {
            animationController.SetTrigger(takeHitAnimationTriggerHash);
            return 2f;
        }
        #endregion

        public override string ToString()
        {
            return $"{base.ToString()} - {Owner}";
        }
    }
}


