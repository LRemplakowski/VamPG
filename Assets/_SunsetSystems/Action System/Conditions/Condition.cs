using Sirenix.OdinInspector;

namespace SunsetSystems.ActionSystem
{
    public abstract class Condition
    {
        [ShowInInspector]
        private bool ConditionMet => IsMet();

        public abstract bool IsMet();
    }
}