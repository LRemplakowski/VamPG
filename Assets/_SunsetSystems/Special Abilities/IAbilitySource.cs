using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public interface IAbilitySource
    {
        IEnumerable<IAbility> GetAbilities();
    }
}
