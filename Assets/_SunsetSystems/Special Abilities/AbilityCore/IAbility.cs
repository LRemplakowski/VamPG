using System;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public interface IAbility
    {
        IAbilityTargetingData GetTargetingData(IAbilityContext context);
        IAbilityCostData GetAbilityCosts(IAbilityContext context);
        IAbilityUIData GetAbilityUIData();
        AbilityCategory GetCategories();

        bool Execute(IAbilityContext context, Action onCompleted = null);
        Awaitable<bool> ExecuteAsync(IAbilityContext context, Action onCompleted = null);

        bool IsContextValidForExecution(IAbilityContext context);
    }
}
