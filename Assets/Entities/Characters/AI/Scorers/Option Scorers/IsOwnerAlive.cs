using Apex.AI;
using Apex.Serialization;

namespace AI.Scorers.Option
{
    public class IsOwnerAlive : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not;

        public override float Score(CreatureContext context)
        {
            return not ^ context.StatsManager.IsAlive() ? score : 0f;
        }
    }
}
