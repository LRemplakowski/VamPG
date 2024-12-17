using System;
using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using UnityEngine;

namespace SunsetSystems.Abilities.Targeting
{
    public class TargetGridStrategy : IAbilityTargetingStrategy
    {
        private readonly IAbilityConfig _ability;
        private Collider _cachedLastHit;

        private event Action ExecuteAbility;

        public TargetGridStrategy(IAbilityConfig ability)
        {
            _ability = ability;
        }

        public void ExecuteTargetSelect(ITargetingContext context)
        {
            Collider hitCollider = context.GetLastRaycastCollider();
            context.GetCurrentGrid().ClearHighlightedCell();
            if (context.IsPointerOverUI() || IsColliderNull(hitCollider) || !hitCollider.TryGetComponent(out GridUnitObject gridCell))
                return;
            if (IsCurrentTarget(hitCollider) && context.IsTargetLocked())
            {
                ExecuteAbility?.Invoke();
            }
            else
            {
                _cachedLastHit = hitCollider;
                context.GetCurrentGrid().HighlightCell(gridCell);
                context.TargetUpdateDelegate().Invoke(gridCell);
                context.TargetLockSetDelegate().Invoke(true);
            }
        }

        public void ExecuteClearTargetLock(ITargetingContext context)
        {
            context.TargetLockSetDelegate().Invoke(false);
        }

        public void ExecutePointerPosition(ITargetingContext context)
        {
            Collider hitCollider = context.GetLastRaycastCollider();
            context.GetCurrentGrid().ClearHighlightedCell();
            if (context.IsPointerOverUI() || IsColliderNull(hitCollider) || !hitCollider.TryGetComponent(out GridUnitObject gridCell))
            {
                context.TargetUpdateDelegate().Invoke(null);
                context.TargetingLineUpdateDelegate().Invoke(false);
            }
            else
            {
                context.GetCurrentGrid().HighlightCell(gridCell);
                context.TargetUpdateDelegate().Invoke(gridCell);
                context.TargetingLineUpdateDelegate().Invoke(false);
            }
        }

        public void ExecuteTargetingBegin(ITargetingContext context)
        {
            context.GetCurrentGrid().ShowCellsInMovementRange(context.GetCurrentCombatant());
            context.TargetUpdateDelegate().Invoke(null);
            context.TargetingLineUpdateDelegate().Invoke(false);
            context.TargetLockSetDelegate().Invoke(false);
        }

        public void ExecuteTargetingEnd(ITargetingContext context)
        {
            context.GetCurrentGrid().HideCellsInMovementRange();
            context.TargetUpdateDelegate().Invoke(null);
            context.TargetingLineUpdateDelegate().Invoke(false);
            context.TargetLockSetDelegate().Invoke(false);
        }


        private bool IsCurrentTarget(Collider target) => _cachedLastHit == target;
        private static bool IsColliderNull(Collider collider) => collider == null;

        public void AddUseAbilityListener(Action listener)
        {
            ExecuteAbility += listener;
        }

        public void RemoveUseAbilityListener(Action listener)
        {
            ExecuteAbility -= listener;
        }
    }
}