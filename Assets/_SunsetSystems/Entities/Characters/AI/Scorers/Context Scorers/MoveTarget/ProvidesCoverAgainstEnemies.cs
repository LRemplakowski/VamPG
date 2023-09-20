using Apex.AI;
using Apex.Serialization;
using UnityEngine;
using System.Collections.Generic;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Combat;

namespace AI.Scorers.Context
{
    public class ProvidesCoverAgainstEnemies : OptionScorerBase<IGridCell, CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;
        [ApexSerialization]
        public float score = 0f;
        [ApexSerialization]
        public LayerMask coverMask;
        [ApexSerialization]
        public float coveredFromCountMultiplier = 1.0f;

        public override float Score(CreatureContext context, IGridCell option)
        {
            bool providesCover = false;
            int count = 0;

            if (CoverDetector.IsPositionNearCover(option, out List<ICover> coverSources))
            {
                List<ICombatant> enemies = new();
                if (context.Owner.Faction.Equals(Faction.Hostile))
                {
                    enemies.AddRange(context.PlayerControlledCombatants);
                    enemies.AddRange(context.FriendlyCombatants);
                }
                else
                {
                    enemies.AddRange(context.EnemyCombatants);
                }
                Vector3 optionPosition = option.WorldPosition;
                foreach (ICombatant enemy in enemies)
                {
                    Vector3 raycastOrigin = enemy.AimingOrigin;
                    float distance = Vector3.Distance(raycastOrigin, optionPosition);
                    Vector3 direction = (raycastOrigin - optionPosition).normalized;
                    RaycastHit[] hits = Physics.RaycastAll(new Ray(raycastOrigin, direction), distance, coverMask);
                    if (hits.Length > 0)
                    {
                        foreach (RaycastHit hit in hits)
                        {
                            ICover coverSource = hit.collider.GetComponent<ICover>();
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