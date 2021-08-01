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
            Vector3 myPos = context.Owner.transform.position;
            Vector3 targetPos = context.CurrentTarget.transform.position;

            return not ^ Vector3.Distance(myPos, targetPos) <= context.StatsManager.GetWeaponMaxRange() ? score : 0f;
        }
    }
}
