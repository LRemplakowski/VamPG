using Apex.AI;
using Apex.Serialization;

namespace AI.Scorers.Option
{
    public class IsGameInCombatState : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not;

        public override float Score(CreatureContext context)
        {
            return not ^ context.IsInCombat ? score : 0f;
        }
    }
}
