using Apex.AI;
using Apex.Serialization;

namespace AI.Scorers.Option
{
    public class IsTargetAlive : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            if (context.CurrentTarget != null)
            {
                return not ^ context.CurrentTarget.GetComponent<StatsManager>().IsAlive() ? score : 0f;
            }
            else
            {
                return not ? score : 0f;
            }
        }
    }
}