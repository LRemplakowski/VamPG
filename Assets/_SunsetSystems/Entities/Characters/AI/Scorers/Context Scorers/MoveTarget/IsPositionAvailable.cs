using Apex.AI;
using Apex.Serialization;
using SunsetSystems.Combat.Grid;

namespace AI.Scorers.Context
{
    public class IsPositionAvailable : OptionScorerBase<IGridCell, CreatureContext>
    {
        [ApexSerialization]
        public bool not;
        [ApexSerialization]
        public float score = 10f;

        public override float Score(CreatureContext context, IGridCell option)
        {
            bool isAvailable = option.IsFree ^ not;
            return isAvailable ? score : 0f;
        }
    }
}
