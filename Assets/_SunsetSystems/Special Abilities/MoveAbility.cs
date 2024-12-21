using System;
using SunsetSystems.Abilities.Execution;
using SunsetSystems.Abilities.Targeting;
using SunsetSystems.ActionSystem;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Inventory;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Abilities
{
    [CreateAssetMenu(fileName = "New Move Ability", menuName = "Sunset Abilities/Move")]
    public class MoveAbility : AbstractAbilityConfig
    {
        private IAbilityExecutionStrategy _executionStrategy;
        private IAbilityTargetingStrategy _targetingStrategy;

        protected override bool HasValidTarget(IAbilityContext context, TargetableEntityType validTargetsMask)
        {
            return IsTargetNotNull(context)
                   && GetTargetHasValidFlag(context, validTargetsMask)
                   && GetTargetAsGridCell(context, out var gridCell)
                   && IsPositionFree(gridCell);

            static bool IsTargetNotNull(IAbilityContext context) => context.TargetObject != null;
            static bool GetTargetHasValidFlag(IAbilityContext context, TargetableEntityType targetMask) => context.TargetObject.IsValidTarget(targetMask);

            static bool GetTargetAsGridCell(IAbilityContext context, out IGridCell cell)
            {
                cell = default;
                if (context.TargetObject is IGridCell gridCell)
                {
                    cell = gridCell;
                    return true;
                }
                return false;
            }

            static bool IsPositionFree(IGridCell cell) => cell.IsFree;
        }

        protected override RangeData GetAbilityRangeData(IAbilityContext context)
        {
            var mover = context.SourceCombatBehaviour.GetContext().MovementManager;
            return new()
            {
                MaxRange = mover.GetCurrentMovementPoints()
            };
        }

        protected override int GetMovementPointCost(IAbilityContext context)
        {
            if (context.TargetObject is not IGridCell position)
            {
                return int.MaxValue;
            }
            var combatBehaviour = context.SourceCombatBehaviour;
            var navManager = combatBehaviour.References.NavigationManager;
            var gridManager = context.GridManager;
            var path = new NavMeshPath();
            navManager.CalculatePath(position.WorldPosition, path);
            var movementCost = Mathf.CeilToInt(path.GetPathLength() / gridManager.GetGridScale()) * _baseMovementCost;
            return movementCost;
        }

        public override IAbilityExecutionStrategy GetExecutionStrategy() => _executionStrategy ??= new MoveStrategy(this);
        public override IAbilityTargetingStrategy GetTargetingStrategy() => _targetingStrategy ??= new DoubleClickGridStrategy(this);
    }
}
