using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SunsetSystems.Abilities;
using SunsetSystems.Animation;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters;
using SunsetSystems.ActionSystem;
using SunsetSystems.Entities.Characters.Navigation;
using SunsetSystems.Equipment;
using SunsetSystems.Inventory;
using UltEvents;
using UnityEngine;
using System;

namespace SunsetSystems.Combat
{
    public class CombatBehaviour : SerializedMonoBehaviour, ICombatant, ITargetable, IMovementPointUser, IActionPointUser, IBloodPointUser
    {
        [Title("Config")]
        [SerializeField]
        private Vector3 _combatNameplateOffset = new Vector3(0, 2, 0);
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
        [SerializeField, Required]
        private IAbilityUser _abilityUser;
        [SerializeField, Required]
        private AnimationManager animationController;

        public bool IsPlayerControlled => Owner.Faction is Faction.PlayerControlled;

        private float _distanceMovedThisTurn = 0f;
        private int _availableActionPoints = 2;
        private Vector3 _positionLastFrame;

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
            hasCoverAnimationBooleanHash = Animator.StringToHash(hasCoverAnimationBoolean);
            _positionLastFrame = transform.position;
        }

        private void OnDisable()
        {
            CombatManager.Instance.CombatRoundBegin -= OnCombatRoundBegin;
            CombatManager.Instance.CombatRoundEnd -= OnCombatRoundEnd;
            CombatManager.Instance.CombatBegin -= OnCombatBegin;
            CombatManager.Instance.CombatEnd -= OnCombatEnd;
        }

        private void Update()
        {
            if (transform.position != _positionLastFrame)
                _distanceMovedThisTurn += Vector3.Distance(transform.position, _positionLastFrame);
            _positionLastFrame = transform.position;
            if (CombatManager.Instance.IsCurrentActiveActor(this))
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
                _availableActionPoints = 2;
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
                _distanceMovedThisTurn = 0f;
                _availableActionPoints = 2;
                _positionLastFrame = transform.position;
                HasMoved = false;
                HasActed = false;
            }
        }

        private void OnCombatRoundEnd(ICombatant currentActor)
        {

        }

        #region ITargetable
        public bool IsValidTarget(TargetableEntityType validTargetsFlag) 
        {
            return References.CreatureData.CreatureType switch
            {
                CreatureType.Mortal => validTargetsFlag.HasFlag(TargetableEntityType.Mortal),
                CreatureType.Ghul => validTargetsFlag.HasFlag(TargetableEntityType.Ghoul),
                CreatureType.Vampire => validTargetsFlag.HasFlag(TargetableEntityType.Vampire),
                CreatureType.Invalid => false,
                _ => false,
            };
        }
        #endregion

        #region ICombatant
        public MonoBehaviour CoroutineRunner => Owner.CoroutineRunner;

        public IWeaponManager WeaponManager => Owner.References.WeaponManager;

        public Vector3 AimingOrigin => RaycastOrigin;
        public Vector3 NameplatePosition => References.Transform.position + _combatNameplateOffset;

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
        [field: SerializeField]
        public UltEvent<ICombatant> OnDamageTaken { get; set; }

        [field: Title("Combat Runtime")]
        [field: ShowInInspector, ReadOnly]
        public bool HasActed { get; private set; }
        [field: ShowInInspector, ReadOnly]
        public bool HasMoved { get; private set; }

        public string ID => Owner.ID;

        public string Name => Owner.Name;

        public Faction Faction => Owner.Faction;

        public ICreatureReferences References => Owner.References;

        public EntityAction PeekCurrentAction => Owner.PeekCurrentAction;

        public bool HasActionsQueued => Owner.HasActionsQueued;

        public Transform Transform => Owner.Transform;

        public bool GetCanMove()
        {
            return SprintRange > Mathf.CeilToInt(_distanceMovedThisTurn);
        }

        #region IActionPointUser
        public int GetCurrentActionPoints()
        {
            return _availableActionPoints;
        }

        public bool UseActionPoints(int ap)
        {
            return true;
        }
        #endregion

        #region IMovementPointUser
        public int GetCurrentMovementPoints()
        {
            return SprintRange - Mathf.CeilToInt(_distanceMovedThisTurn);
        }

        public bool UseMovementPoints(int mp)
        {
            return true;
        }
        #endregion

        #region IBloodPointUser
        public int GetCurrentBloodPoints()
        {
            return 0;
        }

        public bool UseBloodPoints(int bp)
        {
            return true;
        }
        #endregion

        public void TakeDamage(int amount)
        {
            Owner.References.StatsManager.TakeDamage(amount);
            OnDamageTaken?.InvokeSafe(this);
        }

        public int GetAttributeValue(AttributeType attributeType)
        {
            return Owner.References.StatsManager.GetAttribute(attributeType)?.GetValue() ?? 0;
        }

        public Task PerformAction(EntityAction action, bool clearQueue = false)
        {
            return Owner.PerformAction(action, clearQueue);
        }

        public void SignalEndTurn()
        {
            if (CombatManager.Instance.IsCurrentActiveActor(this))
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
            if (currentWeapon == null || currentWeapon.WeaponType is Inventory.AbilityRange.Melee)
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
                    GridUnit nearestUnitInRangeAdjacentToTarget = FindAdjacentGridPosition(target, gridManager, currentGridPosition, MovementRange, References.GetCachedComponent<INavigationManager>());
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
            else if (Vector3.Distance(Transform.position, target.Transform.position) <= currentWeapon.GetRangeData().MaxRange && weaponManager.UseAmmoFromSelectedWeapon(1))
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
            HasActed = true;
            OnUsedActionPoint?.InvokeSafe(this);
            return true;
        }

        public bool IsTargetInRange(ICombatant target)
        {
            return Vector3.Distance(References.Transform.position, target.References.Transform.position) <= weaponManager.GetSelectedWeapon().GetRangeData().MaxRange;
        }

        private static GridUnit FindAdjacentGridPosition(ICombatant target, GridManager grid, Vector3Int currentGridPosition, float movementRange, INavigationManager navigationManager)
        {
            GridUnit unit = null;
            float distance = float.MaxValue;
            List<GridUnit> positionList = grid.GetCellsInRange(currentGridPosition, movementRange, navigationManager, out _);
            if (target != null)
            {
                Vector3Int enemyGridPosition = grid.WorldPositionToGridPosition(target.Transform.position);
                List<GridUnit> walkableCellsNearEnemy = grid.GetCellsInRange(enemyGridPosition, 1.5f, target.References.GetCachedComponent<INavigationManager>(), out _);
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
        #endregion

        #region INamedObject
        public string GetLocalizedName() => References.CreatureData.FullName;
        #endregion

        public override string ToString()
        {
            return $"{base.ToString()} - {Owner}";
        }

        public ICombatContext GetContext()
        {
            throw new NotImplementedException();
        }
    }
}


