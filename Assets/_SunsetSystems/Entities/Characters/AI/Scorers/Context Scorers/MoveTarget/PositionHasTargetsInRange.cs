namespace AI.Scorers.Context
{
    using Apex.AI;
    using Apex.Serialization;
    using SunsetSystems.Entities.Characters;
    using System.Collections.Generic;
    using UnityEngine;

    public class PositionHasTargetsInRange : OptionScorerBase<GridElement, CreatureContext>
    {
        [ApexSerialization]
        public bool not;
        [ApexSerialization]
        public float score = 0f;
        [ApexSerialization]
        public float targetCountMultiplier = 1.0f;

        public override float Score(CreatureContext context, GridElement option)
        {
            bool hasTargets = false;
            List<Creature> targets = new();
            if (context.Owner.Faction.Equals(Faction.Hostile))
            {
                targets.AddRange(context.FriendlyCombatants);
                targets.AddRange(context.PlayerControlledCombatants);
            }
            else
            {
                targets.AddRange(context.EnemyCombatants);
            }

            float range = context.Owner.CurrentWeapon.GetRangeData().maxRange;
            int targetCount = 0;
            Vector3 myPos = option.WorldPosition;
            foreach (Creature c in targets)
            {
                Vector3 targetPos = c.transform.position;
                if (Vector3.Distance(myPos, targetPos) <= range)
                {
                    targetCount++;
                    hasTargets = true;
                }
            }

            if (not)
            {
                return hasTargets ? 0f : score / (targetCount * targetCountMultiplier);
            }
            else
            {
                return hasTargets ? score * targetCount * targetCountMultiplier : 0f;
            }
        }
    }
}