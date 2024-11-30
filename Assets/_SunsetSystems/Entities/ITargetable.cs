using SunsetSystems.Abilities;
using SunsetSystems.Localization;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public interface ITargetable : INamedObject
    {
        ICombatContext CombatContext { get; }

        bool IsFriendlyTowards(ICombatant other);
        bool IsHostileTowards(ICombatant other);
        bool IsMe(ICombatant other);

        bool IsValidEntityType(TargetableEntityType validTargetsFlag);

        void TakeDamage(int damage);
    }
}
