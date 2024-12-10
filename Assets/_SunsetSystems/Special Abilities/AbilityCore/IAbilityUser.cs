using System;
using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public interface IAbilityUser
    {
        IEnumerable<IAbility> GetCoreAbilities();
        IEnumerable<IAbility> GetAllAbilities();

        IAbilityContext GetCurrentAbilityContext();
        bool GetCanAffordAbility(IAbility ability);
        bool GetHasValidAbilityContext(IAbility ability);

        Awaitable<bool> ExecuteAbilityAsync(IAbility ability);
        bool ExecuteAbility(IAbility ability, Action onCompleted = null);

        void SetCurrentTargetObject(ITargetable targetable);
    }
}
