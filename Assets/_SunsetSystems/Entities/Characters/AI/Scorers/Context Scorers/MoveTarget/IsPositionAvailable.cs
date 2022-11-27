using Apex.AI;
using Apex.Serialization;

namespace AI.Scorers.Context
{
    public class IsPositionAvailable : OptionScorerBase<GridElement, CreatureContext>
    {
        [ApexSerialization]
        public bool not;
        [ApexSerialization]
        public float score = 10f;

        public override float Score(CreatureContext context, GridElement option)
        {
            bool isAvailable = !option.Visited.Equals(GridElement.Status.Occupied) ^ not;
            return isAvailable ? score : 0f;
        }
    }
}
