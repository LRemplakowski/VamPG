namespace AI.Scorers.Option
{
    using Apex.AI;
    using Apex.Serialization;
    using UnityEngine;

    public class HasMoveAction : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            return not ^ context.HasMoved ? 0f : score;
        }
    }
}