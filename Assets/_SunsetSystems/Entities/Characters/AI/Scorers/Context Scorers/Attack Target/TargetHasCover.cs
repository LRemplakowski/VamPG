using Apex.AI;
using Apex.Serialization;
using UnityEngine;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Combat;

namespace AI.Scorers.Context
{
    public class TargetHasCover : OptionScorerBase<ICombatant, CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;
        [ApexSerialization]
        public float score = 0f;
        [ApexSerialization]
        public LayerMask coverMask;

        public override float Score(CreatureContext context, ICombatant option)
        {
            bool hasCover = CoverDetector.FiringLineObstructedByCover(context.Owner, option, out ICover _);
            return not ^ hasCover ? score : 0f;
        }
    }
}
