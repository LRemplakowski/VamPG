using Apex.AI;
using Apex.Serialization;
using System;
using UnityEngine;

namespace AI.Scorers.Option
{
    public class HasMoveAction : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            //return not ^ context.HasMoved ? 0f : score;
            throw new NotImplementedException();
        }
    }
}