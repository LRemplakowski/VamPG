using System;
using SunsetSystems.Combat;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public interface IAbility
    {
        IAbilityTargetingData GetTargetingData(IAbilityUser abilityUser);
        IAbilityCostData GetAbilityCosts(IAbilityUser abilityUser, ITargetable target);
        IAbilityUIData GetAbilityUIData();
        AbilityCategory GetCategories();

        bool Execute(IAbilityContext abilityUser, ITargetable target, Action onCompleted = null);
        Awaitable<bool> ExecuteAsync(IAbilityContext context, ITargetable target, Action onCompleted = null);
        bool IsValidTarget(IAbilityUser abilityUser, ITargetable target);
    }
}
