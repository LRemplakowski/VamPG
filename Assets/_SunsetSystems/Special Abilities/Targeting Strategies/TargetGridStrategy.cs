using SunsetSystems.Combat.Grid;
using UnityEngine;

namespace SunsetSystems.Abilities.Targeting
{
    public class TargetGridStrategy : IAbilityTargetingStrategy
    {
        private readonly IAbilityConfig _ability;

        public TargetGridStrategy(IAbilityConfig ability)
        {
            _ability = ability;
        }

        public void ExecutePointerPosition(ITargetingContext context)
        {
            Collider hitCollider = context.GetLastRaycastCollider();
            if (hitCollider.TryGetComponent(out GridUnitObject gridCell))
            {
                context.GetCurrentGrid().HighlightCell(gridCell);
                context.UpdateTargetDelegate().Invoke(gridCell);
                context.UpdateTargetingLineVisibilityDelegate().Invoke(false);
            }
            else
            {
                context.GetCurrentGrid().ClearHighlightedCell();
                context.UpdateTargetDelegate().Invoke(null);
                context.UpdateTargetingLineVisibilityDelegate().Invoke(false);
            }
        }

        public void ExecuteTargetingBegin(ITargetingContext context)
        {
            context.GetCurrentGrid().ShowCellsInMovementRange(context.GetCurrentCombatant());
            context.UpdateTargetDelegate().Invoke(null);
            context.UpdateTargetingLineVisibilityDelegate().Invoke(false);
        }

        public void ExecuteTargetingEnd(ITargetingContext context)
        {
            context.GetCurrentGrid().HideCellsInMovementRange();
            context.UpdateTargetDelegate().Invoke(null);
            context.UpdateTargetingLineVisibilityDelegate().Invoke(false);
        }
    }
}