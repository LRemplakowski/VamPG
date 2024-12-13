using System;
using SunsetSystems.Combat;

namespace SunsetSystems.Abilities
{
    public interface ITargetingContext
    {
        Action<ITargetable> GetTargetHoverDelegate();

        IAbilityConfig GetSelectedAbility();

        bool TryGetTarget(out ITargetable target);
    }
}
