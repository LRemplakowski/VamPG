using Apex.AI;
using Apex.Serialization;
using UnityEngine;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities;

namespace AI.Scorers.Context
{
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
            bool hasCover = CoverDetector.FiringLineObstructedByCover(context.Owner, option, out Cover _);
            return not ^ hasCover ? score : 0f;
        }
    }
}
