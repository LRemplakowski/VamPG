using Apex.AI;
using Apex.Serialization;
using System;
using UnityEngine;

namespace AI.Scorers.Option
{
    public class HasCombatAction : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            //return not ^ context.HasActed ? 0f : score;
            throw new NotImplementedException();
        }
    }
}