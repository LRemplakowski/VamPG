using System;
using SunsetSystems.ActionSystem;
using SunsetSystems.Combat;
using SunsetSystems.Entities;
using UnityEngine;

namespace SunsetSystems.Abilities.Execution
{
    public class ReloadStrategy : IAbilityExecutionStrategy
    {
        public async Awaitable BeginExecute(IAbilityContext context, Action onCompleted)
        {
            await Awaitable.MainThreadAsync();
            var targetCombatContext = (context.TargetObject as IContextProvider<ICombatContext>).GetContext();
            var weaponManager = targetCombatContext.WeaponManager;
            var actionPerformer = context.SourceActionPerformer;
            var reloadAction = new ReloadAction(weaponManager, actionPerformer);
            await actionPerformer.PerformAction(reloadAction);
            onCompleted?.Invoke();
        }
    }
}
