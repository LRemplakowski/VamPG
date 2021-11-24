namespace AI.Scorers.Option
{
    using Apex.AI;
    using Apex.Serialization;
    using Entities.Characters.Actions;
    using UnityEngine;

    public class IsAttacking : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            return not ^ context.Owner.PeekActionFromQueue().GetType().IsAssignableFrom(typeof(HostileAction)) ? score : 0f;
        }
    }
}