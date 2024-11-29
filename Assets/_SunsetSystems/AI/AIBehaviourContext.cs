using System;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Abilities;
using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Game;
using SunsetSystems.Inventory;
using SunsetSystems.Utils.Extensions;
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

        public IAbility SelectedAbility { get; set; }
        public ICombatant SelectedTarget { get; set; }
        public IGridCell SelectedPosition { get; set; }

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

        public bool IsCurrentTargetInAbilityRange()
        {
            return IsInAbilityRange(SelectedAbility, _combatBehaviour, SelectedTarget);
        }

        public IAbilityUser GetAbilityUser() => _combatBehaviour.AbilityUser;
        public ICombatant GetCombatant() => _combatBehaviour;

        public int GetTargetsInWeaponRange()
        {
            return _combatManager.LivingActors.Where(actor => IsHostileToMe(actor) && IsInAbilityRange(SelectedAbility, _combatBehaviour, actor)).Count();
        }

        private bool IsHostileToMe(ITargetable target)
        {
            return target.IsHostileTowards(_combatBehaviour);
        }

        private static bool IsInAbilityRange(IAbility ability, ICombatant attacker, ICombatant target)
        {
            var abilityTargetingData = ability.GetTargetingData(attacker.AbilityUser);
            return abilityTargetingData.GetRangeType() switch
            {
                AbilityRange.Melee => IsInMeleeRange(attacker, target),
                AbilityRange.Ranged => IsInRange(attacker, target, abilityTargetingData.GetRangeData().MaxRange),
                _ => false,
            };

            static bool IsInMeleeRange(ICombatant attacker, ICombatant target) => IsInRange(attacker, target, 1.6f);

            static bool IsInRange(ICombatant attacker, ICombatant target, float range)
            {
                return Vector3.Distance(attacker.Transform.position, target.Transform.position) <= range;
            }
        }

        public bool SelectNextPosition()
        {
            var lastSelectedPosition = _selectedPosition;
            var movementRange = _combatBehaviour.GetRemainingMovement();
            var gridManager = _combatManager.CurrentEncounter.GridManager;
            var positionsInRange = AIHelpers.GetPositionsInRange(_combatBehaviour, movementRange, gridManager);
            if (positionsInRange.Count() > 0)
                lastSelectedPosition = positionsInRange.GetRandom();
            return lastSelectedPosition != _selectedPosition;
        }

        public bool GetHasEnoughActionPoints(IAbility selectedAbility)
        {
            var abilityCost = selectedAbility.GetAbilityCosts(_combatBehaviour.AbilityUser, SelectedTarget);
            return abilityCost.ActionPointCost <= _combatBehaviour.GetRemainingActionPoints();
        }
    }
}
