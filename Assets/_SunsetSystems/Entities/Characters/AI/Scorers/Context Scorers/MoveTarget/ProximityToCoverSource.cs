namespace AI.Scorers.Context
{
    using Apex.AI;
    using Apex.Serialization;
    using SunsetSystems.Combat;
    using SunsetSystems.Combat.Grid;
    using SunsetSystems.Entities;
    using System.Collections.Generic;
    using UnityEngine;

    public class ProximityToCoverSource : OptionScorerBase<IGridCell, CreatureContext>
    {
        [ApexSerialization]
        public float score = 0f;

        public override float Score(CreatureContext context, IGridCell option)
        {
            List<ICover> coverSourcesInGrid = context.CoverSourcesInCombatGrid;
            float distance = float.MaxValue;
            foreach (ICover c in coverSourcesInGrid)
            {
                float f = Vector3.Distance(option.WorldPosition, c.WorldPosition);
                if (f < distance)
                    distance = f;
            }

            return distance > 0f ? score / distance : score;
        }
    }
}