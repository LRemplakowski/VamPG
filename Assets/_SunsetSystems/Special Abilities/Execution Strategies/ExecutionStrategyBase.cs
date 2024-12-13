using System;
using System.Collections.Generic;
using SunsetSystems.ActionSystem;
using UnityEngine;

namespace SunsetSystems.Abilities.Execution
{
    public abstract class ExecutionStrategyBase : IAbilityExecutionStrategy
    {
        public async Awaitable BeginExecute(IAbilityContext context, Action onCompleted)
        {
            var commands = GetCommands();
            IExecutionCommand nextCommand = commands.Dequeue();
            while (nextCommand != null)
            {
                await nextCommand.Execute();
                nextCommand = commands.Dequeue();
            }
            onCompleted?.Invoke();
        }

        protected abstract Queue<IExecutionCommand> GetCommands();
    }

    public interface IExecutionCommand
    {
        Awaitable<bool> Execute();
    }
}