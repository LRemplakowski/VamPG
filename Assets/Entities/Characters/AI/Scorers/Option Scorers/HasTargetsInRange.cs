using Apex.AI;
using Apex.Serialization;
using Entities.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Scorers.Option
{
    public class HasTargetsInRange : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            float weaponsRange = context.StatsManager.GetWeaponMaxRange();
            List<Creature> potentialTargets = new List<Creature>();
            if (context.Owner.Data.Faction.Equals(Faction.Hostile))
            {
                potentialTargets.AddRange(context.FriendlyCombatants);
                potentialTargets.AddRange(context.PlayerControlledCombatants);
            }
            else
            {
                potentialTargets.AddRange(context.EnemyCombatants);
            }
            return not ^ potentialTargets.Exists(target => (Vector3.Distance(context.Owner.CurrentGridPosition.transform.position, target.CurrentGridPosition.transform.position) <= weaponsRange)) ? 
                score : 
                0f;
        }
    }
}
