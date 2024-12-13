using System;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public interface IAbilityExecutionStrategy
    {
        Awaitable BeginExecute(IAbilityContext context, Action onCompleted);
    }
}
