using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Spellbook
{
    public interface IEffect
    {
        AffectedHandler AffectedEffectHandler { get; }

        bool ApplyEffect(IEffectHandler handler);
    }

    public enum AffectedValue
    {
        MaxValue,
        CurrentValue
    }

    public enum EffectModifier
    {
        StaticValue,
        LevelBased,
        RollBased
    }

    public enum FieldType
    {
        Attribute,
        Skill,
        Discipline
    }

    public enum AffectedHandler
    {
        Caster,
        Target
    }
}
