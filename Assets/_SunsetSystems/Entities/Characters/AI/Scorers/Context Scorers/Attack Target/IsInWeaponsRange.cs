using Apex.AI;
using Apex.Serialization;
using SunsetSystems.Entities.Characters;
using UnityEngine;
using SunsetSystems.Entities.Interfaces;

namespace AI.Scorers.Context
{
    public class IsInWeaponsRange : OptionScorerBase<ICombatant, CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;
        [ApexSerialization]
        public float score = 0f;
        [ApexSerialization]
        public float effectiveRangeMultiplier = 2.0f;

        public override float Score(CreatureContext context, ICombatant option)
        {
            Vector3 myPos = context.Owner.References.Transform.position;
            Vector3 targetPos = option.References.Transform.position;
            float distance = Vector3.Distance(myPos, targetPos);

            return not ^ (distance <= context.Owner.CurrentWeapon.GetRangeData().maxRange) ?
                (distance <= context.Owner.CurrentWeapon.GetRangeData().optimalRange ? score * effectiveRangeMultiplier : score) :
                0f;
        }
    }
}
