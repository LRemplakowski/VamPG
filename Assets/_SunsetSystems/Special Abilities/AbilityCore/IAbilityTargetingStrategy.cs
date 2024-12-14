using System;
using SunsetSystems.Combat;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public interface IAbilityTargetingStrategy
    {
        void ExecutePointerPosition(ITargetingContext context);
        void ExecuteTargetingBegin(ITargetingContext context);
        void ExecuteTargetingEnd(ITargetingContext context);
    }
}
