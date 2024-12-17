using System;
using SunsetSystems.Combat;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public interface IAbilityTargetingStrategy
    {
        void ExecuteTargetSelect(ITargetingContext context);
        void ExecuteClearTargetLock(ITargetingContext context);
        void ExecutePointerPosition(ITargetingContext context);
        void ExecuteTargetingBegin(ITargetingContext context);
        void ExecuteTargetingEnd(ITargetingContext context);

        void AddUseAbilityListener(Action listener);
        void RemoveUseAbilityListener(Action listener);
    }
}
