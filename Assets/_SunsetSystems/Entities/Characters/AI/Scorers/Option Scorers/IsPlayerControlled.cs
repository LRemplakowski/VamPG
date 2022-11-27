using Apex.AI;
using Apex.Serialization;

namespace AI.Scorers.Option
{
    public class IsPlayerControlled : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            return not ^ context.IsPlayerControlled ? score : 0f;            
        }
    }
}