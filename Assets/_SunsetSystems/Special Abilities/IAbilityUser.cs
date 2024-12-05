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

        IAbilityContext GetAbilityContext(ITargetable target);

        Awaitable<bool> ExecuteAbilityAsync(IAbility ability, ITargetable target);
        bool ExecuteAbility(IAbility ability, ITargetable target, Action onCompleted = null);

        void SetCurrentTargetObject(ITargetable targetable);
        void SetCurrentTargetPosition(IGridCell position);
    }
}
