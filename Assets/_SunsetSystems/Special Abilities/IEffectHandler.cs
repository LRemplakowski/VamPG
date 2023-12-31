namespace SunsetSystems.Spellbook
{
    public interface IEffectHandler
    {
        void HandleEffect(IEffect effect, IMagicUser caster);
    }
}
