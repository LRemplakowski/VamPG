using System;
using Sirenix.OdinInspector;
using SunsetSystems.Abilities;
using SunsetSystems.Combat.UI;
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
        private IAbilityConfig _selectedAbility;

        private ITargetingContext _targetingContext;

        private void Awake()
        {
            ActionBarUI.OnAbilitySelected += OnCombatActionSelected;
            CombatManager.OnCombatRoundBegin += OnCombatRoundBegin;
            CombatManager.OnCombatRoundEnd += OnCombatRoundEnd;
            CombatManager.OnFullTurnCompleted += OnFullTurnCompleted;
        }

        private void OnDestroy()
        {
            ActionBarUI.OnAbilitySelected -= OnCombatActionSelected;
            CombatManager.OnCombatRoundBegin -= OnCombatRoundBegin;
            CombatManager.OnCombatRoundEnd -= OnCombatRoundEnd;
            CombatManager.OnFullTurnCompleted -= OnFullTurnCompleted;
        }

        public void Initialize(ITargetingContext targetingContext)
        {
            SetTargetingContext(targetingContext);
        }

        public void OnCombatRoundBegin(ICombatant actor)
        {
            if (actor.GetContext().IsPlayerControlled)
            {
                CleanupBeforeActionChange(GetSelectedAbility());
                SetSelectedAbility(_defaultAbility);
                HandleNewSelectedAction(GetSelectedAbility());
            }
        }

        public void OnFullTurnCompleted(ICombatant actor)
        {
            if (actor.GetContext().IsPlayerControlled)
            {
                CleanupBeforeActionChange(GetSelectedAbility());
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
            CleanupBeforeActionChange(GetSelectedAbility());
            SetSelectedAbility(newAbility);
            HandleNewSelectedAction(GetSelectedAbility());
        }

        private void CleanupBeforeActionChange(IAbilityConfig ability)
        {
            if (ability == null)
                return;
            var targetingStrategy = ability.GetTargetingStrategy()
                                    ?? throw new NullReferenceException($"Ability {ability} provided a null Targeting Strategy!");
            targetingStrategy.RemoveUseAbilityListener(ExecuteSelectedAction);
            targetingStrategy.ExecuteTargetingEnd(GetTargetingContext());
        }

        private void HandleNewSelectedAction(IAbilityConfig ability)
        {
            if (ability == null)
                return;
            var targetingStrategy = ability.GetTargetingStrategy()
                                    ?? throw new NullReferenceException($"Ability {ability} provided a null Targeting Strategy!");
            targetingStrategy.RemoveUseAbilityListener(ExecuteSelectedAction);
            targetingStrategy.AddUseAbilityListener(ExecuteSelectedAction);
            targetingStrategy.ExecuteTargetingBegin(GetTargetingContext());
        }

        private void ExecuteSelectedAction()
        {
            var selectedAbility = GetSelectedAbility();
            _targetingContext.GetCurrentCombatant().GetContext().AbilityUser.ExecuteAbility(selectedAbility, OnFinishedExecution);
            selectedAbility.GetTargetingStrategy().RemoveUseAbilityListener(ExecuteSelectedAction);

            void OnFinishedExecution()
            {
                OnCombatActionSelected(GetSelectedAbility());
            }
        }

        public void ExecuteAction(IAbilityConfig ability)
        {
            combatManager.CurrentActiveActor.GetContext().AbilityUser.ExecuteAbility(ability);
        }

        public IAbilityConfig GetSelectedAbility() => _selectedAbility;
        private void SetSelectedAbility(IAbilityConfig ability) => _selectedAbility = ability;

        private ITargetingContext GetTargetingContext() => _targetingContext;
        private void SetTargetingContext(ITargetingContext context) => _targetingContext = context;
    }
}
