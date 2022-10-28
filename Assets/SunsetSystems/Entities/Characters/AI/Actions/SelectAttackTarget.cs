namespace AI.Actions
{
    using Apex.AI;
    using SunsetSystems.Entities.Characters;
    using System.Collections.Generic;

    public class SelectAttackTarget : ActionWithOptions<Creature>
    {
        public override void Execute(IAIContext context)
        {
            CreatureContext c = context as CreatureContext;
            Creature self = c.Owner;

            List<Creature> potentialTargets = new List<Creature>();
            if (self.Data.faction.Equals(Faction.Hostile))
            {
                potentialTargets.AddRange(c.FriendlyCombatants);
                potentialTargets.AddRange(c.PlayerControlledCombatants);
            }
            else
            {
                potentialTargets.AddRange(c.EnemyCombatants);
            }

            Creature target = this.GetBest(c, potentialTargets);

            if (target != null)
            {
                c.CurrentTarget = target;
            }
        }
    }
}