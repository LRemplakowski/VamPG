using System;
using SunsetSystems.Combat;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public interface IAbilityTargetingStrategy
    {
        Awaitable BeginExecute(ITargetingContext context);
    }
}
