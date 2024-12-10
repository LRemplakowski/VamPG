using System;
using Sirenix.OdinInspector;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters;
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
        [field: ShowInInspector, ReadOnly]
        public SelectedCombatActionData SelectedActionData { get; private set; }

        private Collider gridHit, targetableHit;

        public void OnCombatRoundBegin(ICombatant actor)
        {
            if (actor.GetContext().IsPlayerControlled)
            {
                this.SelectedActionData = new(_defaultAbility);
                HandleNewSelectedAction(SelectedActionData);
            }
        }

        public void OnFullTurnCompleted(ICombatant actor)
        {
            if (actor.GetContext().IsPlayerControlled)
            {
                this.SelectedActionData = new(_defaultAbility);
                HandleNewSelectedAction(SelectedActionData);
            }
        }

        public void OnCombatRoundEnd(ICombatant actor)
        {
            if (actor.GetContext().IsPlayerControlled)
            {
                CleanupBeforeActionChange(SelectedActionData);
            }
        }

        public void OnCombatActionSelected(SelectedCombatActionData actionData)
        {
            if (SelectedActionData.AbilityData != actionData.AbilityData)
            {
                CleanupBeforeActionChange(SelectedActionData);
                HandleNewSelectedAction(actionData);
            }
            this.SelectedActionData = actionData;
        }

        private void CleanupBeforeActionChange(SelectedCombatActionData action)
        {
            var categories = action.AbilityData.GetCategories();
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

        private void HandleNewSelectedAction(SelectedCombatActionData action)
        {
            var categories = action.AbilityData.GetCategories();
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

        public void ExecuteAction(SelectedCombatActionData action)
        {
            combatManager.CurrentActiveActor.GetContext().AbilityUser.ExecuteAbility(action.AbilityData);
        }

        public void SetLastGridHit(Collider gridCollider)
        {
            gridHit = gridCollider;
        }

        public void SetLastTargetableHit(Collider targetableCollider)
        {
            targetableHit = targetableCollider;
        }
    }

    [Serializable]
    public struct SelectedCombatActionData
    {
        [SerializeField]
        public IAbility AbilityData;

        public SelectedCombatActionData(IAbility AbilityData)
        {
            this.AbilityData = AbilityData;
        }
    }
}
