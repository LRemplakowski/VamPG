using Apex.AI;
using Apex.Serialization;

namespace AI.Scorers.Option
{
    public class IsMyTurn : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            return not ^ context.Owner.Equals(TurnCombatManager.Instance.CurrentActiveActor) ? score : 0f;
        }
    }
}