using Sirenix.OdinInspector;

namespace SunsetSystems.Spellbook
{
    public class CreatureSpellEffectHandler : SerializedMonoBehaviour, IEffectHandler
    {
        public void HandleEffect(IEffect effect, IAbilityUser caster)
        {
            effect.ApplyEffect(this);
        }

        public EffectHandlerSceneContext GetContext()
        {
            return new(this);
        }
    }
}
