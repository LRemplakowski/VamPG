namespace AI.Scorers.Option
{
    using Apex.AI;
    using Apex.Serialization;
    using UnityEngine;

    public class IsCurrentEntityActionIdle : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            return not ^ context.Owner.PickActionQueue().GetType().Equals(typeof(Idle)) ? score : 0f;
        }
    }
}
