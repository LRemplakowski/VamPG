using SunsetSystems.Spellbook;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities.Interfaces
{
    public interface ITargetable
    {
        bool IsFriendlyTowards(ICombatant other);
        bool IsHostileTowards(ICombatant other);
        bool IsMe(ICombatant other);

        IEffectHandler EffectHandler { get; }
    }
}
