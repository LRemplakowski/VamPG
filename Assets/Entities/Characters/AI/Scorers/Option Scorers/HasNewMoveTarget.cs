namespace AI.Scorers.Option
{
    using Apex.AI;
    using Apex.Serialization;
    using UnityEngine;

    public class HasNewMoveTarget : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            return not ^ !context.Owner.CurrentGridPosition.Equals(context.CurrentMoveTarget) ? score : 0f;
        }
    }
}