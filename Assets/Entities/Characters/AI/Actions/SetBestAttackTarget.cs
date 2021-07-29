using Apex.AI;
using System.Collections.Generic;

namespace AI.Actions
{
    public class SetBestAttackTarget : ActionWithOptions<Creature>
    {
        public override void Execute(IAIContext context)
        {
            CreatureContext c = context as CreatureContext;
            Creature self = c.Owner;

            List<Creature> potentialTargets = new List<Creature>();
            if (self.Faction.Equals(Faction.Hostile))
            {
                potentialTargets.AddRange(c.FriendlyCombatants);
                potentialTargets.AddRange(c.PlayerControlledCombatants);
            } 
            else
            {
                potentialTargets.AddRange(c.EnemyCombatants);
            }

            Creature target = this.GetBest(context, potentialTargets);

            if (target != null)
            {
                c.CurrentTarget = target;
            }
        }
    }
}