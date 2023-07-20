using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace AI.Scorers.Option
{
    public class IsCurrentTargetInRange : ContextualScorerBase<CreatureContext>
    {
        [ApexSerialization]
        public bool not = false;

        public override float Score(CreatureContext context)
        {
            if (context.CurrentTarget == null)
                return 0;
            Vector3 myPos = context.Owner.References.Transform.position;
            Vector3 targetPos = context.CurrentTarget.References.Transform.position;

            return not ^ Vector3.Distance(myPos, targetPos) <= context.Owner.CurrentWeapon.GetRangeData().maxRange ? score : 0f;
        }
    }
}
