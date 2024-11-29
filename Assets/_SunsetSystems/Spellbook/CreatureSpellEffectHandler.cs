using Sirenix.OdinInspector;

namespace SunsetSystems.Abilities
{
    public class CreatureSpellEffectHandler : SerializedMonoBehaviour, IEffectHandler
    {
        public void HandleEffect(IEffect effect, ISpellbookManager caster)
        {
            effect.ApplyEffect(this);
        }

        public EffectHandlerSceneContext GetContext()
        {
            return new(this);
        }
    }
}
