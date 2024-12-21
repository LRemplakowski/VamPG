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
            if (ValidateTarget(context, out ICombatant target) is false)
            {
                ClearTargetingDelegates(context);
                DisableExecutionUI(context);
                return;
            }
            ICombatant current = context.GetCurrentCombatant();
            context.TargetUpdateDelegate().Invoke(target.References.Targetable);
            var abilityRange = _ability.GetTargetingData(context.GetAbilityContext()).GetRangeData();
            if (IsTargetInRange(current, target, in abilityRange))
            {
                current.References.NavigationManager.FaceDirectionAfterMovementFinished(target.Transform.position);
                var targetingLineRenderer = context.GetTargetingLineRenderer();
                targetingLineRenderer.SetPosition(1, target.AimingOrigin);
                context.TargetingLineUpdateDelegate().Invoke(true);
                context.TargetLockSetDelegate().Invoke(true);
                var executionUI = context.GetExecutionUI();
                executionUI.RegisterConfirmationCallback(TriggerExecution);
                executionUI.UpdateShowInterface(true, () => context.CanExecuteAbility(_ability));
            }
        }

        public void ExecuteClearTargetLock(ITargetingContext context)
        {
            ClearTargetingDelegates(context);
            DisableExecutionUI(context);
        }

        public void ExecutePointerPosition(ITargetingContext context)
        {
            if (ValidateTarget(context, out ICombatant target) is false)
            {
                ClearTargetingDelegates(context);
                return;
            }
            context.TargetUpdateDelegate().Invoke(target.References.Targetable);
            var abilityRange = _ability.GetTargetingData(context.GetAbilityContext()).GetRangeData();
            ICombatant current = context.GetCurrentCombatant();
            if (IsTargetInRange(current, target, in abilityRange))
            {
                var targetingLineRenderer = context.GetTargetingLineRenderer();
                targetingLineRenderer.SetPosition(1, target.AimingOrigin);
                context.TargetingLineUpdateDelegate().Invoke(true);
                current.References.NavigationManager.FaceDirectionAfterMovementFinished(target.Transform.position);
            }
        }

        public void ExecuteTargetingBegin(ITargetingContext context)
        {
            ClearTargetingDelegates(context);
            DisableExecutionUI(context);
            context.GetTargetingLineRenderer().SetPosition(0, context.GetCurrentCombatant().AimingOrigin);
        }

        public void ExecuteTargetingEnd(ITargetingContext context)
        {
            ClearTargetingDelegates(context);
            DisableExecutionUI(context);
        }

        private void ClearTargetingDelegates(ITargetingContext context)
        {
            context.TargetUpdateDelegate().Invoke(null);
            context.TargetingLineUpdateDelegate().Invoke(false);
            context.TargetLockSetDelegate().Invoke(false);
        }

        private void DisableExecutionUI(ITargetingContext context)
        {
            var executionUI = context.GetExecutionUI();
            executionUI.UnregisterConfirmationCallback(TriggerExecution);
            executionUI.UpdateShowInterface(false, () => false);
        }

        private void TriggerExecution()
        {
            OnExecutionTriggered?.Invoke();
        }

        private static bool ValidateTarget(ITargetingContext context, out ICombatant target)
        {
            target = default;
            var collider = context.GetLastRaycastCollider();
            if (collider == null)
                return false;
            if (collider.TryGetComponent(out ICreature targetCreature) is false)
                return false;
            target = targetCreature.References.CombatBehaviour;
            return target.GetContext().IsAlive;
        }

        private static bool IsTargetInRange(ICombatant attacker, ICombatant target, in RangeData abilityRange)
        {
            Vector3 attackerPosition = attacker.Transform.position;
            Vector3 targetPosition = target.Transform.position;
            return Vector3.Distance(attackerPosition, targetPosition) <= abilityRange.MaxRange;
        }
    }
}
