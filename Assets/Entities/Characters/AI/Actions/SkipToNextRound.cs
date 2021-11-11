namespace AI.Actions
{
    using Apex.AI;

    public class SkipToNextRound : ActionBase<CreatureContext>
    {
        public override void Execute(CreatureContext context)
        {
            TurnCombatManager.Instance.NextRound();
        }
    }
}