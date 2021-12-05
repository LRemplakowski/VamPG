using Apex.AI;
using Apex.Serialization;
using Entities.Characters;

namespace AI.Scorers.Context
{
    public class IsTargetAlive : OptionScorerBase<Creature, CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;
        [ApexSerialization]
        public float score = 0f;

        public override float Score(CreatureContext context, Creature option)
        {
            return not ^ option.GetComponent<StatsManager>().IsAlive() ? score : 0f;
        }
    }
}