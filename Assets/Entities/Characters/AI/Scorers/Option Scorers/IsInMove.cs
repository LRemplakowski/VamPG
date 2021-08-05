namespace AI.Scorers.Option
{
    using Apex.AI;
    using Apex.Serialization;
    using UnityEngine;

    public class IsInMove : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            return not ^ context.Owner.PeekActionFromQueue().GetType().Equals(typeof(Move)) ? score : 0f;
        }
    }
}