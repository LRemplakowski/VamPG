namespace SunsetSystems.Spellbook
{
    public interface IEffect
    {
        AffectedHandler AffectedEffectHandler { get; }

        bool ApplyEffect(IEffectHandler handler);
        bool ValidateTarget(EffectHandlerSceneContext context);
    }

    public enum AffectedHandler
    {
        Caster,
        Target
    }
}
