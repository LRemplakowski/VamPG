using Sirenix.OdinInspector;

namespace SunsetSystems.Spellbook
{
    public class CreatureSpellEffectHandler : SerializedMonoBehaviour, IEffectHandler
    {
        public void HandleEffect(IEffect effect, IMagicUser caster)
        {
            effect.ApplyEffect(this);
        }

        public EffectHandlerSceneContext GetContext()
        {
            return new();
        }
    }
}
