using System;
using SunsetSystems.ActionSystem;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    [CreateAssetMenu(fileName = "New Move Ability", menuName = "Sunset Abilities/Move")]
    public class MoveAbility : BaseAbility
    {
        protected override bool CanExecuteAbility(IAbilityContext context)
        {
            return base.CanExecuteAbility(context) && context.TargetCharacter is IGridCell;
        }

        protected override async Awaitable DoExecuteAbility(IAbilityContext context, Action onCompleted)
        {
            var moveAction = new Move(context.SourceCombatBehaviour, context.TargetCharacter as IGridCell, context.GridManager);
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
    }
}
