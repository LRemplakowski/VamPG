namespace AI.Scorers.Option
{
    using Apex.AI;
    using Apex.Serialization;
    using UnityEngine;

    public class HasActionsQueued : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            return not ^ context.Owner.HasActionsInQueue() ? score : 0f;
        }
    }
}
