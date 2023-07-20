using Apex.AI;
using SunsetSystems.Combat;
using System;

namespace AI.Actions
{
    public class SkipToNextRound : ActionBase<CreatureContext>
    {
        public override void Execute(CreatureContext context)
        {
            //context.Owner.CombatBehaviour.HasActed = true;
            //context.Owner.CombatBehaviour.HasMoved = true;
            throw new NotImplementedException();
        }
    }
}