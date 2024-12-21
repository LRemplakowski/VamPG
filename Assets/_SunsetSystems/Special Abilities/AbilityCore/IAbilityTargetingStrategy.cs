using System;
using SunsetSystems.Combat;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public interface IAbilityTargetingStrategy
    {
        event Action OnExecutionTriggered;

        void ExecuteSetTargetLock(ITargetingContext context);
        void ExecuteClearTargetLock(ITargetingContext context);
        void ExecutePointerPosition(ITargetingContext context);
        void ExecuteTargetingBegin(ITargetingContext context);
        void ExecuteTargetingEnd(ITargetingContext context);
    }
}
