using System;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public interface IAbilityConfig
    {
        IAbilityTargetingData GetTargetingData(IAbilityContext context);
        IAbilityCostData GetAbilityCosts(IAbilityContext context);
        IAbilityUIData GetAbilityUIData();
        AbilityCategory GetCategories();

        bool IsContextValidForExecution(IAbilityContext context);

        bool Execute(IAbilityContext context, Action onCompleted = null);
        Awaitable<bool> ExecuteAsync(IAbilityContext context, Action onCompleted = null);

        IAbilityExecutionStrategy GetExecutionStrategy();
        IAbilityTargetingStrategy GetTargetingStrategy();
    }
}
