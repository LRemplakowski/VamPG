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

        [Title("References")]
        [SerializeField]
        private Transform _raycastOrigin;
        public Vector3 RaycastOrigin => _raycastOrigin.position;
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

        public bool HasActionsQueued => Owner.HasActionsQueued;

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
            if (HasMoved)
                return false;
            GridManager gridManager = CombatManager.Instance.CurrentEncounter.GridManager;
            GridUnit gridUnit = gridManager[gridPosition];
            if (IsPlayerControlled)
            {
                if (gridUnit.IsInMoveRange && !HasMoved)
                {
                    HasMoved = true;
                    OnChangedGridPosition?.Invoke();
                    CombatManager.Instance.CurrentEncounter.GridManager.HideCellsInMovementRange();
                    _ = PerformAction(new Move(this, gridUnit, gridManager));
                }
                else if (gridUnit.IsInSprintRange && !HasMoved && !HasActed)
                {
                    HasActed = true;
                    HasMoved = true;
                    OnChangedGridPosition?.Invoke();
                    CombatManager.Instance.CurrentEncounter.GridManager.HideCellsInMovementRange();
                    _ = PerformAction(new Move(this, gridUnit, gridManager));
                }
            }
            else
            {
                HasMoved = true;
                OnChangedGridPosition?.Invoke();
                _ = PerformAction(new Move(this, gridUnit, gridManager));
            }
            return false;
        }

        public bool AttackCreatureUsingCurrentWeapon(ICombatant target)
        {
            if (HasActed)
                return false;
            HasActed = true;
            _ = PerformAction(new Attack(target, this));
            return true;
        }

        /// <summary>
        /// Instructs combatant to perform attack animation with current weapon.
        /// </summary>
        /// <returns>Animation duration</returns>
        public float PerformAttackAnimation()
        {
            animationController.SetTrigger(attackAnimationTriggerHash);
            return 1f;
        }

        /// <summary>
        /// Instructs combatant to perform getting hit animation.
        /// </summary>
        /// <returns>Animation duration</returns>
        public float PerformTakeHitAnimation()
        {
            animationController.SetTrigger(takeHitAnimationTriggerHash);
            return 1f;
        }
        #endregion

        public override string ToString()
        {
            return $"{base.ToString()} - {Owner}";
        }
    }
}


