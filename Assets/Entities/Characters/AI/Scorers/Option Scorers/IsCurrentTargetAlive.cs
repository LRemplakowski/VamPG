using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace AI.Scorers.Option
{
    public class IsCurrentTargetAlive : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            return not ^ (context.CurrentTarget != null && context.CurrentTarget.GetComponent<StatsManager>().IsAlive()) ? score : 0f;
        }
    }
}
