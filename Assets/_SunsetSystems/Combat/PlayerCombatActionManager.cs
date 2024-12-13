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
        public IAbilityConfig _selectedAbility;

        private Collider gridHit, targetableHit;

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
            var categories = ability.GetCategories();
            switch (categories)
            {
                case AbilityCategory when categories.HasFlag(AbilityCategory.Movement):
                    break;
                case AbilityCategory when categories.HasFlag(AbilityCategory.Attack):
                    break;
                case AbilityCategory when categories.HasFlag(AbilityCategory.Support):
                    break;
                case AbilityCategory when categories.HasFlag(AbilityCategory.Magical):
                    break;
                case AbilityCategory when categories.HasFlag(AbilityCategory.Debuff):
                    break;
            }
        }

        private void HandleNewSelectedAction(IAbilityConfig ability)
        {
            var categories = ability.GetCategories();
            switch (categories)
            {
                case AbilityCategory when categories.HasFlag(AbilityCategory.Movement):
                    break;
                case AbilityCategory when categories.HasFlag(AbilityCategory.Attack):
                    break;
                case AbilityCategory when categories.HasFlag(AbilityCategory.Support):
                    break;
                case AbilityCategory when categories.HasFlag(AbilityCategory.Magical):
                    break;
                case AbilityCategory when categories.HasFlag(AbilityCategory.Debuff):
                    break;
            }
        }

        public void ExecuteAction(IAbilityConfig ability)
        {
            combatManager.CurrentActiveActor.GetContext().AbilityUser.ExecuteAbility(ability);
        }

        public void SetLastGridHit(Collider gridCollider)
        {
            gridHit = gridCollider;
        }

        public void SetLastTargetableHit(Collider targetableCollider)
        {
            targetableHit = targetableCollider;
        }

        private IAbilityConfig GetSelectedAbility() => _selectedAbility;
        private void SetSelectedAbility(IAbilityConfig ability) => _selectedAbility = ability;
    }
}
