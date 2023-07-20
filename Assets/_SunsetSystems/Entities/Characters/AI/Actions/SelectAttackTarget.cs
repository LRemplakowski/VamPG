using Apex.AI;
using SunsetSystems.Entities.Characters;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Entities.Interfaces;

namespace AI.Actions
{
    public class SelectAttackTarget : ActionWithOptions<ICombatant>
    {
        public override void Execute(IAIContext context)
        {
            CreatureContext c = context as CreatureContext;
            ICombatant self = c.Owner;

            List<ICombatant> potentialTargets = new();
            if (self.Faction.Equals(Faction.Hostile))
            {
                potentialTargets.AddRange(c.FriendlyCombatants);
                potentialTargets.AddRange(c.PlayerControlledCombatants);
            }
            else
            {
                potentialTargets.AddRange(c.EnemyCombatants);
            }

            ICombatant target = this.GetBest(c, potentialTargets);
            Debug.Log($"Selected target {target.Name}");
            if (target != null)
            {
                c.CurrentTarget = target;
            }
        }
    }
}