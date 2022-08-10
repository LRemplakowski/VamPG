namespace AI.Scorers.Context
{
    using Apex.AI;
    using Apex.Serialization;
    using Entities.Characters;

    public class IsCurrentTarget : OptionScorerBase<Creature, CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;
        [ApexSerialization]
        public float score = 0f;

        public override float Score(CreatureContext context, Creature option)
        {
            if (context.CurrentTarget)
            {
                return not ^ context.CurrentTarget.Equals(option) ? score : 0f;
            }
            return not ? score : 0f;
        }
    }
}
