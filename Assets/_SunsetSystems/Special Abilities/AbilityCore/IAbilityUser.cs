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
        IEnumerable<IAbilityConfig> GetCoreAbilities();
        IEnumerable<IAbilityConfig> GetAllAbilities();

        IAbilityContext GetCurrentAbilityContext();
        bool GetCanAffordAbility(IAbilityConfig ability);
        bool GetHasValidAbilityContext(IAbilityConfig ability);

        Awaitable<bool> ExecuteAbilityAsync(IAbilityConfig ability);
        bool ExecuteAbility(IAbilityConfig ability, Action onCompleted = null);

        void SetCurrentTargetObject(ITargetable targetable);
    }
}
