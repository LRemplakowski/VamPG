using SunsetSystems.Abilities;

namespace SunsetSystems.Combat
{
    public interface ITargetable
    {
        bool IsFriendlyTowards(ICombatant other);
        bool IsHostileTowards(ICombatant other);
        bool IsMe(ICombatant other);

        bool IsValidEntityType(TargetableEntityType validTargetsFlag);

        IEffectHandler EffectHandler { get; }
    }
}
