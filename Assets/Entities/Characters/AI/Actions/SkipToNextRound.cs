namespace AI.Actions
{
    using Apex.AI;
    using SunsetSystems.Management;

    public class SkipToNextRound : ActionBase<CreatureContext>
    {
        public override void Execute(CreatureContext context)
        {
            References.Get<TurnCombatManager>().NextRound();
        }
    }
}