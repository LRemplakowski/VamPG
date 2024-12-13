using System;
using SunsetSystems.ActionSystem;
using UnityEngine;

namespace SunsetSystems.Abilities.Execution
{
    public class MoveStrategy : IAbilityExecutionStrategy
    {
        private readonly MoveAbility _ability;

        public MoveStrategy(MoveAbility ability)
        {
            _ability = ability;
        }

        public async Awaitable BeginExecute(IAbilityContext context, Action onCompleted)
        {
            var moveAction = new MoveAbilityAction(_ability, context);
            await context.SourceActionPerformer.PerformAction(moveAction);
            onCompleted?.Invoke();
        }
    }
}
