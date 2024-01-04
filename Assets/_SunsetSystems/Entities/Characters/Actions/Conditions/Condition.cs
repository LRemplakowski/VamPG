using Sirenix.OdinInspector;

namespace SunsetSystems.Entities.Characters.Actions.Conditions
{
    public abstract class Condition
    {
        [ShowInInspector]
        private bool ConditionMet => IsMet();

        public abstract bool IsMet();
    }
}