using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Combat;
using UnityEngine;

namespace SunsetSystems.Abilities.Targeting
{
    public class ShowConfirmButtonAfterTargetSelection : IAbilityTargetingStrategy
    {
        private readonly IAbilityConfig _ability;

        public ShowConfirmButtonAfterTargetSelection(IAbilityConfig ability)
        {
            _ability = ability;
        }

        public async Awaitable BeginExecute(ITargetingContext context)
        {
            while (_ability == context.GetSelectedAbility())
            {
                if (context.TryGetTarget(out ITargetable target))
                {
                    context.GetTargetHoverDelegate().Invoke(target);
                }
                await Awaitable.NextFrameAsync();
            }
        }
    }
}
