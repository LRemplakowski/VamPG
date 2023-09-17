namespace AI.Scorers.Context
{
    using Apex.AI;
    using Apex.Serialization;
    using SunsetSystems.Combat;
    using SunsetSystems.Entities;
    using System.Collections.Generic;
    using UnityEngine;

    public class ProximityToCoverSource : OptionScorerBase<GridElement, CreatureContext>
    {
        [ApexSerialization]
        public float score = 0f;

        public override float Score(CreatureContext context, GridElement option)
        {
            List<ICover> coverSourcesInGrid = context.CoverSourcesInCombatGrid;
            float distance = float.MaxValue;
            foreach (ICover c in coverSourcesInGrid)
            {
                float f = Vector3.Distance(option.transform.position, c.WorldPosition);
                if (f < distance)
                    distance = f;
            }

            return distance > 0f ? score / distance : score;
        }
    }
}