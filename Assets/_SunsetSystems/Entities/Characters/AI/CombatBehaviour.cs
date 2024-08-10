using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SunsetSystems.Animation;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Combat.UI;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Characters.Navigation;
using SunsetSystems.Equipment;
using SunsetSystems.Inventory;
using SunsetSystems.Spellbook;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public class CombatBehaviour : SerializedMonoBehaviour, ICombatant, ITargetable
    {
        [Title("Config")]
        [SerializeField]
        private Vector3 _combatNameplateOffset = new Vector3(0, 2, 0);
        [SerializeField]
        private float defaultRaycastOriginY = 1.5f;
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
        private AnimationManager animationController;

        public bool IsPlayerControlled => Owner.Faction is Faction.PlayerControlled;

        #region Unity Messages
        private void OnEnable()
        {
            CombatManager.Instance.CombatBegin += OnCombatBegin;
            CombatManager.Instance.CombatRoundBegin += OnCombatRoundBegin;
            CombatManager.Instance.CombatRoundEnd += OnCombatRoundEnd;
            CombatManager.Instance.CombatEnd += OnCombatEnd;
            WeaponSetSelectorButton.OnWeaponSelected += OnWeaponSelected;
        }

        private void Start()
        {
            hasCoverAnimationBooleanHash = Animator.StringToHash(hasCoverAnimationBoolean);
        }

        private void OnDisable()
        {
            CombatManager.Instance.CombatRoundBegin -= OnCombatRoundBegin;
            CombatManager.Instance.CombatRoundEnd -= OnCombatRoundEnd;
            CombatManager.Instance.CombatBegin -= OnCombatBegin;
            CombatManager.Instance.CombatEnd -= OnCombatEnd;
            WeaponSetSelectorButton.OnWeaponSelected -= OnWeaponSelected;
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

        private void OnWeaponSelected(SelectedWeapon weapon)
        {
            if (CombatManager.Instance.CurrentActiveActor.Equals(this))
            {
                weaponManager.SetSelectedWeapon(weapon);
                OnWeaponChanged?.InvokeSafe(this);
            }
        }

        #region ITargetable
        public IEffectHandler EffectHandler => throw new System.NotImplementedException();

        public bool IsFriendlyTowards(ICombatant other)
        {
            throw new System.NotImplementedException();
        }

        public bool IsHostileTowards(ICombatant other)
        {
            throw new System.NotImplementedException();
        }

        public bool IsMe(ICombatant other)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region ICombatant
        public MonoBehaviour CoroutineRunner => Owner.CoroutineRunner;
        public IWeapon CurrentWeapon => Owner.References.WeaponManager.GetSelectedWeapon();

        public IWeapon PrimaryWeapon => Owner.References.WeaponManager.GetPrimaryWeapon();

        public IWeapon SecondaryWeapon => Owner.References.WeaponManager.GetSecondaryWeapon();

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
        public UltEvent<ICombatant> OnWeaponChanged { get; set; }
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

        public bool TakeDamage(int amount)
        {
            Owner.References.StatsManager.TakeDamage(amount);
            OnDamageTaken?.InvokeSafe(this);
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
            HasActed = true;
            return true;
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

        /// <summary>
        /// Instructs combatant to perform attack animation with current weapon.
        /// </summary>
        /// <returns>Animation duration</returns>
        public float PerformAttackAnimation()
        {
            animationController.PlayFireWeaponAnimation();
            return 1f;
        }

        /// <summary>
        /// Instructs combatant to perform getting hit animation.
        /// </summary>
        /// <returns>Animation duration</returns>
        public float PerformTakeHitAnimation()
        {
            animationController.PlayTakeHitAnimation();
            return 1f;
        }
        #endregion

        public override string ToString()
        {
            return $"{base.ToString()} - {Owner}";
        }
    }
}


