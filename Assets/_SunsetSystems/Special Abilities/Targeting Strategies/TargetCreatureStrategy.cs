using System;
using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.Abilities.Targeting
{
    public class TargetCreatureStrategy : IAbilityTargetingStrategy
    {
        private readonly IAbilityConfig _ability;

        public event Action OnExecutionTriggered;

        public TargetCreatureStrategy(IAbilityConfig ability)
        {
            _ability = ability;
        }

        public void ExecuteSetTargetLock(ITargetingContext context)
        {
            Collider collider = context.GetLastRaycastCollider();
            if (collider == null)
                return;
            LineRenderer targetingLineRenderer = context.GetTargetingLineRenderer();
            if (collider.gameObject.TryGetComponent(out ICreature targetCreature) is false)
            {
                ClearTargetingDelegates(context);
                return;
            }
            ICombatant current = context.GetCurrentCombatant();
            ICombatant target = targetCreature.References.CombatBehaviour;
            if (target.GetContext().IsAlive is false)
            {
                ClearTargetingDelegates(context);
                return;
            }
            context.TargetUpdateDelegate().Invoke(target.References.Targetable);
            var abilityRange = _ability.GetTargetingData(context.GetAbilityContext()).GetRangeData();
            if (IsTargetInRange(current, target, in abilityRange))
            {
                targetingLineRenderer.SetPosition(1, target.AimingOrigin);
                context.TargetingLineUpdateDelegate().Invoke(true);
                context.TargetLockSetDelegate().Invoke(true);
                current.References.NavigationManager.FaceDirectionAfterMovementFinished(target.Transform.position);
                var executionUI = context.GetExecutionUI();
                executionUI.RegisterConfirmationCallback(TriggerExecution);
                executionUI.SetExectuionValidationDelegate(CanPerformAction);
                executionUI.SetActive(true);
            }

            bool CanPerformAction()
            {
                var user = context.GetCurrentCombatant().GetContext().AbilityUser;
                bool canAfford = user.GetCanAffordAbility(_ability);
                bool hasValidContext = _ability.IsContextValidForExecution(user.GetCurrentAbilityContext());
                return canAfford && hasValidContext;
            }
        }

        public void ExecuteClearTargetLock(ITargetingContext context)
        {
            ClearTargetingDelegates(context);
            var executionUI = context.GetExecutionUI();
            executionUI.UnregisterConfirmationCallback(TriggerExecution);
            executionUI.SetActive(false);
        }

        public void ExecutePointerPosition(ITargetingContext context)
        {
            Collider collider = context.GetLastRaycastCollider();
            if (collider == null)
                return;
            LineRenderer targetingLineRenderer = context.GetTargetingLineRenderer();
            if (collider.gameObject.TryGetComponent(out ICreature targetCreature) is false)
            {
                ClearTargetingDelegates(context);
                return;
            }
            ICombatant current = context.GetCurrentCombatant();
            ICombatant target = targetCreature.References.CombatBehaviour;
            if (target.GetContext().IsAlive is false)
            {
                ClearTargetingDelegates(context);
                return;
            }
            context.TargetUpdateDelegate().Invoke(target.References.Targetable);
            var abilityRange = _ability.GetTargetingData(context.GetAbilityContext()).GetRangeData();
            if (IsTargetInRange(current, target, in abilityRange))
            {
                targetingLineRenderer.SetPosition(1, target.AimingOrigin);
                context.TargetingLineUpdateDelegate().Invoke(true);
                current.References.NavigationManager.FaceDirectionAfterMovementFinished(target.Transform.position);
            }
        }

        public void ExecuteTargetingBegin(ITargetingContext context)
        {
            ClearTargetingDelegates(context);
            context.GetTargetingLineRenderer().SetPosition(0, context.GetCurrentCombatant().AimingOrigin);
            var executionUI = context.GetExecutionUI();
            executionUI.UnregisterConfirmationCallback(TriggerExecution);
            executionUI.SetActive(false);
        }

        public void ExecuteTargetingEnd(ITargetingContext context)
        {
            ClearTargetingDelegates(context);
            var executionUI = context.GetExecutionUI();
            executionUI.UnregisterConfirmationCallback(TriggerExecution);
            executionUI.SetActive(false);
        }

        private void ClearTargetingDelegates(ITargetingContext context)
        {
            context.TargetUpdateDelegate().Invoke(null);
            context.TargetingLineUpdateDelegate().Invoke(false);
            context.TargetLockSetDelegate().Invoke(false);
        }

        private void TriggerExecution()
        {
            OnExecutionTriggered?.Invoke();
        }

        private static bool IsTargetInRange(ICombatant attacker, ICombatant target, in RangeData abilityRange)
        {
            Vector3 attackerPosition = attacker.Transform.position;
            Vector3 targetPosition = target.Transform.position;
            return Vector3.Distance(attackerPosition, targetPosition) <= abilityRange.MaxRange;
        }
    }
}
