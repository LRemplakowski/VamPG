namespace AI.Scorers.Option
{
    using Apex.AI;
    using Apex.Serialization;
    using SunsetSystems.Entities;
    using System.Collections.Generic;
    using UnityEngine;

    public class IsInCover : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            bool result = not ^ (context.Owner.CurrentGridPosition != null &&
                    CoverDetector.IsPositionNearCover(context.Owner.CurrentGridPosition, out List<Cover> coverSources));
            Debug.Log("is in cover? " + result);
            return result ? score : 0f;
        }
    }
}
