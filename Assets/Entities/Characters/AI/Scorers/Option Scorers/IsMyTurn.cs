using Apex.AI;
using Apex.Serialization;
using SunsetSystems.Management;

namespace AI.Scorers.Option
{
    public class IsMyTurn : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            return not ^ context.Owner.Equals(References.Get<TurnCombatManager>().CurrentActiveActor) ? score : 0f;
        }
    }
}