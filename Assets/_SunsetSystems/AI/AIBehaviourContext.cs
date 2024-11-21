using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Game;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.AI
{
    public class AIBehaviourContext : SerializedMonoBehaviour
    {
        [Title("Config")]
        [SerializeField]
        private ICombatant _combatBehaviour;
        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private IGridCell _selectedPosition;

        private CombatManager _combatManager;
        private GameManager _gameManager;

        private void Start()
        {
            _combatManager = CombatManager.Instance;
            _gameManager = GameManager.Instance;
        }

        public bool IsMyTurn()
        {
            return _combatManager.IsCurrentActiveActor(_combatBehaviour);
        }

        public bool CanMove()
        {
            return _combatBehaviour.HasActionsQueued is false && _combatBehaviour.GetCanMove();
        }

        public bool IsTurnMode()
        {
            return _gameManager.IsCurrentState(GameState.Combat);
        }

        public bool IsInCover()
        {
            return _combatBehaviour.IsInCover;
        }

        public int GetTargetsInWeaponRange()
        {
            var currentWeapon = _combatBehaviour.WeaponManager.GetSelectedWeapon();
            return _combatManager.LivingActors.Where(actor => IsHostileToMe(actor) && IsInWeaponRange(currentWeapon, actor)).Count();
        }

        private bool IsHostileToMe(ICombatant target)
        {
            return _combatBehaviour.Faction switch
            {
                Faction.None => false,
                Faction.Hostile => target.Faction == Faction.Friendly && target.Faction == Faction.PlayerControlled,
                Faction.Neutral => false,
                Faction.Friendly => target.Faction == Faction.Hostile,
                Faction.PlayerControlled => target.Faction == Faction.Hostile,
                _ => false,
            };
        }

        private bool IsInWeaponRange(IWeapon weapon, ICombatant target)
        {
            return weapon.WeaponType switch
            {
                AbilityRange.Melee => IsInMeleeRange(target),
                AbilityRange.Ranged => IsInRange(target, weapon.GetRangeData().MaxRange),
                _ => false,
            };

            bool IsInMeleeRange(ICombatant target)
            {
                return IsInRange(target, 1.6f);
            }

            bool IsInRange(ICombatant target, float range)
            {
                return Vector3.Distance(_combatBehaviour.Transform.position, target.Transform.position) <= range;
            }
        }

        public bool SelectNextPosition()
        {
            var lastSelectedPosition = _selectedPosition;
            var movementRange = _combatBehaviour.GetRemainingMovement();
            var gridManager = _combatManager.CurrentEncounter.GridManager;
            var positionsInRange = AIHelpers.GetPositionsInRange(_combatBehaviour, movementRange, gridManager);
            return lastSelectedPosition != _selectedPosition;
        }
    }
}
