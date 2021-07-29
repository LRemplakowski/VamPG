using Apex.AI;
using Apex.Serialization;

namespace AI.Scorers.Option
{
    public class HasTarget : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            return not ^ context.CurrentTarget != null ? score : 0f;
        }
    }
}
