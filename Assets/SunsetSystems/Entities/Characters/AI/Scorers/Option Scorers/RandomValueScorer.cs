namespace AI.Scorers.Option
{
    using Apex.AI;
    using Apex.Serialization;
    using UnityEngine;

    public class RandomValueScorer : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public int minScoreMultiplier, maxScoreMultiplier;
        private static readonly System.Random random = new();

        public override float Score(CreatureContext context)
        {
            return maxScoreMultiplier > minScoreMultiplier ? score * random.Next(minScoreMultiplier, maxScoreMultiplier + 1) : -1f;
        }
    }
}