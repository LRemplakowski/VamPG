namespace AI.Scorers.Option
{
    using Apex.AI;
    using Apex.Serialization;
    using Entities.Characters.Actions;
    using UnityEngine;

    public class IsCurrentEntityActionIdle : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            return not ^ context.Owner.PeekActionFromQueue().GetType().Equals(typeof(Idle)) ? score : 0f;
        }
    }
}
