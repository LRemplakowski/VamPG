namespace AI.Scorers.Option
{
    using Apex.AI;
    using Apex.Serialization;
    using SunsetSystems.Entities.Characters.Actions;
    using UnityEngine;

    public class IsInMove : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            return not ^ context.Owner.PeekActionFromQueue().GetType().IsAssignableFrom(typeof(Move)) ? score : 0f;
        }
    }
}