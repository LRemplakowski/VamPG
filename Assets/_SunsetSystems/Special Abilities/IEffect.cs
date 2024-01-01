namespace SunsetSystems.Spellbook
{
    public interface IEffect
    {
        AffectedHandler AffectedEffectHandler { get; }

        bool ApplyEffect(IEffectHandler handler);
    }

    public enum AffectedHandler
    {
        Caster,
        Target
    }
}
