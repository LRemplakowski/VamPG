namespace AI.Actions
{
    using Apex.AI;
    using SunsetSystems.Entities.Characters;
    using System.Collections.Generic;
    using UnityEngine;

    public class SelectAttackTarget : ActionWithOptions<Creature>
    {
        public override void Execute(IAIContext context)
        {
            CreatureContext c = context as CreatureContext;
            Creature self = c.Owner;

            List<Creature> potentialTargets = new();
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
            Debug.Log($"Selected target {target.Data.FullName}");
            if (target != null)
            {
                c.CurrentTarget = target;
            }
        }
    }
}