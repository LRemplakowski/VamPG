namespace AI.Scorers.Context
{
    using Apex.AI;
    using Apex.Serialization;
    using UnityEngine;
    using System.Collections.Generic;
    using SunsetSystems.Entities.Characters;
    using SunsetSystems.Entities.Cover;

    public class TargetHasCover : OptionScorerBase<Creature, CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;
        [ApexSerialization]
        public float score = 0f;
        [ApexSerialization]
        public LayerMask coverMask;

        public override float Score(CreatureContext context, Creature option)
        {
            bool hasCover = false;
            if (CoverDetector.IsPositionNearCover(option.CurrentGridPosition, out List<Cover> coverSources))
            {
                Vector3 raycastOrigin = context.Behaviour.RaycastOrigin;
                Vector3 optionPosition = option.transform.position;
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
                            hasCover = true;
                            break;
                        }
                    }
                }
            }
            return not ^ hasCover ? score : 0f;
        }
    }
}
