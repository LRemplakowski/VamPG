namespace SunsetSystems.Spellbook
{
    public interface IEffectHandler
    {
        EffectHandlerSceneContext GetContext();
        void HandleEffect(IEffect effect, IMagicUser caster);
    }
}
