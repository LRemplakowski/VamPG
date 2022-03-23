namespace AI.Scorers.Context
{
    using Apex.AI;
    using Apex.Serialization;
    using SunsetSystems.Dice;
    using System;
    using Entities.Characters;

    public class HasAttackPoolAdvantage : OptionScorerBase<Creature, CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;
        [ApexSerialization]
        public float score = 0f;
        [ApexSerialization]
        public float multiplier = 1f;

        public override float Score(CreatureContext context, Creature option)
        {
            AttributeSkillPool targetDefense = option.GetComponent<StatsManager>().GetDefensePool();
            AttributeSkillPool myAttack = context.StatsManager.GetDefensePool();
            int poolDifference = myAttack.GetPoolSize() - targetDefense.GetPoolSize();
            if (multiplier > 0f)
            {
                return not ^ (poolDifference > 0) ? Math.Abs(poolDifference) * score * multiplier : 0f;
            }
            else
            {
                return not ^ (poolDifference > 0) ? score : 0f;
            }
        }
    }
}
