using System;
using SunsetSystems.ActionSystem;
using SunsetSystems.Inventory;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Abilities
{
    [CreateAssetMenu(fileName = "New Move Ability", menuName = "Sunset Abilities/Move")]
    public class MoveAbility : BaseAbility
    {
        protected override bool HasValidTarget(IAbilityContext context)
        {
            return IsPositionNotNull(context) && IsPositionFree(context);

            static bool IsPositionNotNull(IAbilityContext context) => context.TargetPosition != null;
            static bool IsPositionFree(IAbilityContext context) => context.TargetPosition.IsFree;
        }

        protected override async Awaitable DoExecuteAbility(IAbilityContext context, Action onCompleted)
        {
            var moveAction = new Move(context.SourceCombatBehaviour, context.TargetPosition, context.GridManager);
            await context.SourceActionPerformer.PerformAction(moveAction);
            onCompleted?.Invoke();
        }

        protected override RangeData GetAbilityRangeData(IAbilityContext context)
        {
            return new()
            {
                OptimalRange = context.SourceCombatBehaviour.MovementRange,
                MaxRange = context.SourceCombatBehaviour.SprintRange
            };
        }

        protected override int GetMovementPointCost(IAbilityContext context)
        {
            var position = context.TargetPosition;
            if (position == null)
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
