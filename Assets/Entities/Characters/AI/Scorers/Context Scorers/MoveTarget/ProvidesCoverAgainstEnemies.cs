namespace AI.Scorers.Context
{
    using Apex.AI;
    using Apex.Serialization;
    using UnityEngine;
    using System.Collections.Generic;
    using Entities.Characters;
    using Entities.Cover;

    public class ProvidesCoverAgainstEnemies : OptionScorerBase<GridElement, CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;
        [ApexSerialization]
        public float score = 0f;
        [ApexSerialization]
        public LayerMask coverMask;
        [ApexSerialization]
        public float coveredFromCountMultiplier = 1.0f;

        public override float Score(CreatureContext context, GridElement option)
        {
            bool providesCover = false;
            int count = 0;

            if (CoverDetector.IsPositionNearCover(option, out List<Cover> coverSources))
            {
                List<Creature> enemies = new List<Creature>();
                if (context.Owner.Faction.Equals(Faction.Hostile))
                {
                    enemies.AddRange(context.PlayerControlledCombatants);
                    enemies.AddRange(context.FriendlyCombatants);
                }
                else
                {
                    enemies.AddRange(context.EnemyCombatants);
                }
                Vector3 optionPosition = option.transform.position;
                foreach (Creature enemy in enemies)
                {
                    Vector3 raycastOrigin = enemy.GetComponent<CombatBehaviour>().RaycastOrigin;
                    float distance = Vector3.Distance(raycastOrigin, optionPosition);
                    Vector3 direction = (raycastOrigin - optionPosition).normalized;
                    RaycastHit[] hits = Physics.RaycastAll(new Ray(raycastOrigin, direction), distance, coverMask);
                    if (hits.Length > 0)
                    {
                        foreach (RaycastHit hit in hits)
                        {
                            Cover coverSource = hit.collider.GetComponent<Cover>();
                            if (coverSources.Contains(coverSource))
                            {
                                providesCover = true;
                                count++;
                            }
                        }
                    }
                }
            }
            
            if (not)
            {
                return providesCover ? 0f : score / (count * coveredFromCountMultiplier);
            }
            else
            {
                return providesCover ? score * count * coveredFromCountMultiplier : 0f;
            }
        }
    }
}