using System;
using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.Abilities.Targeting
{
    public class TargetCreatureStrategy : IAbilityTargetingStrategy
    {
        private static readonly Color TargetInRangeColor = Color.red;
        private static readonly Color TargetOutOfRangeColor = Color.gray;

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
            bool isTargetInRange = IsTargetInRange(current, target, in abilityRange);
            current.References.NavigationManager.FaceDirectionAfterMovementFinished(target.Transform.position);
            context.TargetLockSetDelegate().Invoke(true);
            var executionUI = context.GetExecutionUI();
            if (isTargetInRange)
            {
                if (CanShowTargetingLine(in abilityRange))
                    ShowTargetingLine(context, target.AimingOrigin, TargetInRangeColor);
                executionUI.RegisterConfirmationCallback(TriggerExecution);
                executionUI.UpdateShowInterface(true, () => context.CanExecuteAbility(_ability));
            }
            else
            {
                if (CanShowTargetingLine(in abilityRange))
                    ShowTargetingLine(context, target.AimingOrigin, TargetOutOfRangeColor);
                executionUI.UnregisterConfirmationCallback(TriggerExecution);
                executionUI.UpdateShowInterface(true, () => false);
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
            current.References.NavigationManager.FaceDirectionAfterMovementFinished(target.Transform.position);
            if (CanShowTargetingLine(in abilityRange))
            {
                var targetingLineColor = IsTargetInRange(current, target, in abilityRange) ? TargetInRangeColor : TargetOutOfRangeColor;
                ShowTargetingLine(context, target.AimingOrigin, in targetingLineColor);
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

        private void ShowTargetingLine(ITargetingContext context, in Vector3 target, in Color lineColor)
        {
            var targetingLineRenderer = context.GetTargetingLineRenderer();
            targetingLineRenderer.SetPosition(1, target);
            targetingLineRenderer.startColor = lineColor;
            targetingLineRenderer.endColor = lineColor;
            context.TargetingLineUpdateDelegate().Invoke(true);
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

        private static bool CanShowTargetingLine(in RangeData abilityRange) => abilityRange.MaxRange > 1;
    }
}
