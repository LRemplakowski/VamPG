using UnityEngine;

namespace SunsetSystems.Abilities
{
    public interface IAbilityTargetingStrategy
    {
        Awaitable<bool> BeginExecute(ITargetingContext context);
    }
}
