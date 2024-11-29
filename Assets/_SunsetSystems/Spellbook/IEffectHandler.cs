namespace SunsetSystems.Abilities
{
    public interface IEffectHandler
    {
        EffectHandlerSceneContext GetContext();
        void HandleEffect(IEffect effect, ISpellbookManager caster);
    }
}
