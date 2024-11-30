using System;
using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Combat;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public interface IAbilityUser
    {
        IEnumerable<IAbility> GetAllAbilities();

        Awaitable<bool> ExecuteAbilityAsync(IAbility ability, ITargetable target);
        bool ExecutAbility(IAbility ability, ITargetable target, Action onCompleted = null);
    }
}
