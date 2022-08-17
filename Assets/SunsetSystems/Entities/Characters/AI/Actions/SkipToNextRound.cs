using Apex.AI;
using SunsetSystems.Combat;

namespace AI.Actions
{
    public class SkipToNextRound : ActionBase<CreatureContext>
    {
        public override void Execute(CreatureContext context)
        {
            CombatManager.Instance.NextRound();
        }
    }
}