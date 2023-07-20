using Apex.AI;
using Apex.Serialization;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Interfaces;

namespace AI.Scorers.Context
{
    public class IsCurrentTarget : OptionScorerBase<ICombatant, CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;
        [ApexSerialization]
        public float score = 0f;

        public override float Score(CreatureContext context, ICombatant option)
        {
            if (context.CurrentTarget != null)
            {
                return not ^ context.CurrentTarget.Equals(option) ? score : 0f;
            }
            return not ? score : 0f;
        }
    }
}
