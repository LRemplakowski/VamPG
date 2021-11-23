using Apex.AI;
using Apex.Serialization;
using Systems.Management;

namespace AI.Scorers.Option
{
    public class IsMyTurn : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            return not ^ context.Owner.Equals(ReferenceManager.GetManager<TurnCombatManager>().CurrentActiveActor) ? score : 0f;
        }
    }
}