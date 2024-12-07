using System;
using SunsetSystems.ActionSystem;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Inventory;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Abilities
{
    [CreateAssetMenu(fileName = "New Move Ability", menuName = "Sunset Abilities/Move")]
    public class MoveAbility : BaseAbility
    {
        protected override bool HasValidTarget(IAbilityContext context, TargetableEntityType validTargetsMask)
        {
            return IsTargetNotNull(context) && GetTargetIsPosition(context, out var gridCell) && IsPositionFree(gridCell);

            static bool IsTargetNotNull(IAbilityContext context) => context.TargetObject != null;
            static bool GetTargetIsPosition(IAbilityContext context, out IGridCell cell)
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

        protected override async Awaitable DoExecuteAbility(IAbilityContext context, Action onCompleted)
        {
            throw new NotImplementedException();
            //var targetPosition = context.TargetObject as IGridCell;
            //var moveAction = new Move(context.SourceCombatBehaviour, targetPosition, context.GridManager);
            //await context.SourceActionPerformer.PerformAction(moveAction);
            //onCompleted?.Invoke();
        }

        protected override RangeData GetAbilityRangeData(IAbilityContext context)
        {
            if (context.SourceCombatBehaviour is not IMovementPointUser mover)
                return new();
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
    }
}
