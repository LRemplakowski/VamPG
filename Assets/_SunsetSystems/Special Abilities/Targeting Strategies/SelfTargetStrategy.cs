using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Abilities.Targeting
{
    public class SelfTargetStrategy : IAbilityTargetingStrategy
    {
        private IAbilityConfig _ability;

        public event Action OnExecutionTriggered;

        public SelfTargetStrategy(IAbilityConfig abilityConfig)
        {
            _ability = abilityConfig;
        }

        public void ExecuteClearTargetLock(ITargetingContext context)
        {
            
        }

        public void ExecutePointerPosition(ITargetingContext context)
        {
            context.TargetUpdateDelegate().Invoke(context.GetCurrentCombatant());
        }

        public void ExecuteTargetingBegin(ITargetingContext context)
        {
            context.TargetUpdateDelegate().Invoke(context.GetCurrentCombatant());
            context.GetExecutionUI().SetActive(false);
            context.TargetingLineUpdateDelegate().Invoke(false);
            context.TargetLockSetDelegate().Invoke(false);
        }

        public void ExecuteTargetingEnd(ITargetingContext context)
        {
            
        }

        public void ExecuteSetTargetLock(ITargetingContext context)
        {
            context.TargetUpdateDelegate().Invoke(context.GetCurrentCombatant());
        }
    }
}
