using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public interface IAbilityUser
    {
        IEnumerable<IAbility> GetAllAbilities();
    }
}
