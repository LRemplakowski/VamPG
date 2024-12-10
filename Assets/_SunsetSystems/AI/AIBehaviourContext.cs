using System;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Abilities;
using SunsetSystems.ActorResources;
using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities;
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
        [SerializeField]
        private IContextProvider<ICombatContext> _combatContextSource;
        [SerializeField]
        private IFactionMember _thisFaction;
        [SerializeField]
        private IMovementPointUser _movementPointUser;
        [SerializeField]
        private IActionPointUser _actionPointUser;
        [SerializeField]
        private IBloodPointUser _bloodPointUser;

        private ICombatContext CombatContext => _combatContextSource.GetContext();
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
            return _combatBehaviour.HasActionsQueued is false && CombatContext.MovementManager.GetCanMove();
        }

        public bool IsTurnMode()
        {
            return _gameManager.IsCurrentState(GameState.Combat);
        }

        public bool IsInCover()
        {
            return CombatContext.IsInCover;
        }

        public bool IsCurrentTargetInAbilityRange()
        {
            return IsInAbilityRange(SelectedAbility, CombatContext, SelectedTarget);
        }

        public IAbilityUser GetAbilityUser() => CombatContext.AbilityUser;
        public ICombatant GetCombatant() => _combatBehaviour;

        public int GetTargetsInWeaponRange()
        {
            return _combatManager.LivingActors.Where(actor => IsHostileToMe(actor) && IsInAbilityRange(SelectedAbility, CombatContext, actor)).Count();
        }

        private bool IsHostileToMe(ITargetable target)
        {
            if (target is not IFactionMember factionMember)
                return false;
            return factionMember.IsHostileTowards(_thisFaction);
        }

        private static bool IsInAbilityRange(IAbility ability, ICombatContext attacker, ICombatant target)
        {
            var abilityUser = attacker.AbilityUser;
            abilityUser.SetCurrentTargetObject(target);
            var abilityTargetingData = ability.GetTargetingData(abilityUser.GetCurrentAbilityContext());
            return abilityTargetingData.GetRangeType() switch
            {
                AbilityRange.Melee => IsInMeleeRange(attacker, target),
                AbilityRange.Ranged => IsInRange(attacker, target, abilityTargetingData.GetRangeData().MaxRange),
                _ => false,
            };

            static bool IsInMeleeRange(ICombatContext attacker, ICombatant target) => IsInRange(attacker, target, 1.6f);

            static bool IsInRange(ICombatContext attacker, ICombatant target, float range)
            {
                return Vector3.Distance(attacker.Transform.position, target.Transform.position) <= range;
            }
        }

        public bool SelectNextPosition()
        {
            var lastSelectedPosition = _selectedPosition;
            var movementRange = _movementPointUser.GetCurrentMovementPoints();
            var gridManager = _combatManager.CurrentEncounter.GridManager;
            var positionsInRange = AIHelpers.GetPositionsInRange(_combatBehaviour, movementRange, gridManager);
            if (positionsInRange.Count() > 0)
            {
                lastSelectedPosition = positionsInRange.GetRandom();
            }
            return lastSelectedPosition != _selectedPosition;
        }

        public bool GetHasEnoughActionPoints(IAbility selectedAbility)
        {
            var abilityUser = CombatContext.AbilityUser;
            abilityUser.SetCurrentTargetObject(SelectedTarget);
            return abilityUser.GetCanAffordAbility(selectedAbility);
        }
    }
}
