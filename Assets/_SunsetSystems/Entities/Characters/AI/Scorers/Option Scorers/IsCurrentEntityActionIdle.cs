using Apex.AI;
using Apex.Serialization;
using SunsetSystems.Entities.Characters.Actions;
using System;
using UnityEngine;

namespace AI.Scorers.Option
{
    public class IsCurrentEntityActionIdle : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            //return not ^ context.Owner.PeekActionFromQueue().GetType().Equals(typeof(Idle)) ? score : 0f;
            throw new NotImplementedException();
        }
    }
}
