using System;
using Sirenix.OdinInspector;
using SunsetSystems.Abilities;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public class PlayerCombatActionManager : SerializedMonoBehaviour
    {
        [SerializeField, Required]
        private CombatManager combatManager;
        [SerializeField, Required]
        private IAbilityConfig _defaultAbility;
        [ShowInInspector, ReadOnly]
        public IAbilityConfig SelectedAbility;

        private ITargetingContext _targetingContext;

        public void Initialize(ITargetingContext targetingContext)
        {
            SetTargetingContext(targetingContext);
        }

        public void OnCombatRoundBegin(ICombatant actor)
        {
            if (actor.GetContext().IsPlayerControlled)
            {
                SetSelectedAbility(_defaultAbility);
                HandleNewSelectedAction(GetSelectedAbility());
            }
        }

        public void OnFullTurnCompleted(ICombatant actor)
        {
            if (actor.GetContext().IsPlayerControlled)
            {
                SetSelectedAbility(_defaultAbility);
                HandleNewSelectedAction(GetSelectedAbility());
            }
        }

        public void OnCombatRoundEnd(ICombatant actor)
        {
            if (actor.GetContext().IsPlayerControlled)
            {
                CleanupBeforeActionChange(GetSelectedAbility());
            }
        }

        public void OnCombatActionSelected(IAbilityConfig newAbility)
        {
            if (GetSelectedAbility() != newAbility)
            {
                CleanupBeforeActionChange(GetSelectedAbility());
                SetSelectedAbility(newAbility);
                HandleNewSelectedAction(GetSelectedAbility());
            }
        }

        private void CleanupBeforeActionChange(IAbilityConfig ability)
        {
            ability.GetTargetingStrategy().ExecuteTargetingEnd(GetTargetingContext());
        }

        private void HandleNewSelectedAction(IAbilityConfig ability)
        {
            ability.GetTargetingStrategy().ExecuteTargetingBegin(GetTargetingContext());
        }

        public void ExecuteAction(IAbilityConfig ability)
        {
            combatManager.CurrentActiveActor.GetContext().AbilityUser.ExecuteAbility(ability);
        }

        private IAbilityConfig GetSelectedAbility() => SelectedAbility;
        private void SetSelectedAbility(IAbilityConfig ability) => SelectedAbility = ability;

        private ITargetingContext GetTargetingContext() => _targetingContext;
        private void SetTargetingContext(ITargetingContext context) => _targetingContext = context;
    }
}
