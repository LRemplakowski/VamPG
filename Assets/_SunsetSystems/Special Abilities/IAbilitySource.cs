using System.Collections.Generic;

namespace SunsetSystems.Abilities
{
    public interface IAbilitySource
    {
        IEnumerable<IAbility> GetAbilities();
    }
}
