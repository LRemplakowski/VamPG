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
        private IAbility _defaultAbility;
        [ShowInInspector, ReadOnly]
        public IAbility _selectedAbility;

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

        public void OnCombatActionSelected(IAbility newAbility)
        {
            if (GetSelectedAbility() != newAbility)
            {
                CleanupBeforeActionChange(GetSelectedAbility());
                SetSelectedAbility(newAbility);
                HandleNewSelectedAction(GetSelectedAbility());
            }
        }

        private void CleanupBeforeActionChange(IAbility ability)
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

        private void HandleNewSelectedAction(IAbility ability)
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

        public void ExecuteAction(IAbility ability)
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

        private IAbility GetSelectedAbility() => _selectedAbility;
        private void SetSelectedAbility(IAbility ability) => _selectedAbility = ability;
    }
}
