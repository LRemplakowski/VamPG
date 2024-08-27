using SunsetSystems.Spellbook;

namespace SunsetSystems.Combat
{
    public interface ITargetable
    {
        bool IsFriendlyTowards(ICombatant other);
        bool IsHostileTowards(ICombatant other);
        bool IsMe(ICombatant other);

        IEffectHandler EffectHandler { get; }
    }
}
