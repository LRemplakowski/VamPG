namespace AI.Scorers.Context
{
    using Apex.AI;
    using Apex.Serialization;
    using Entities.Characters;
    using UnityEngine;

    public class IsInWeaponsRange : OptionScorerBase<Creature, CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;
        [ApexSerialization]
        public float score = 0f;
        [ApexSerialization]
        public float effectiveRangeMultiplier = 2.0f;

        public override float Score(CreatureContext context, Creature option)
        {
            Vector3 myPos = context.Owner.transform.position;
            Vector3 targetPos = option.transform.position;
            float distance = Vector3.Distance(myPos, targetPos);

            return not ^ (distance <= context.StatsManager.GetWeaponMaxRange()) ?
                (distance <= context.StatsManager.GetWeaponEffectiveRange() ? score * effectiveRangeMultiplier : score) :
                0f;
        }
    }
}
