using Apex.AI;
using Apex.Serialization;
using System;
using UnityEngine;

namespace AI.Scorers.Option
{
    public class HasActionsQueued : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            //return not ^ context.Owner.HasActionsInQueue() ? score : 0f;
            throw new NotImplementedException();
        }
    }
}
