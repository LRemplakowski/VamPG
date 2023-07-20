namespace AI.Actions
{
    using Apex.AI;
    using SunsetSystems.Entities.Characters;
    using SunsetSystems.Entities.Characters.Actions;
    using SunsetSystems.Entities.Characters.Interfaces;
    using SunsetSystems.Entities.Interfaces;
    using System.Collections.Generic;

    public class AttackCurrentTarget : ActionBase<CreatureContext>
    {
        public override void Execute(CreatureContext context)
        {
            ICombatant owner = context.Owner;
            ICombatant target = context.CurrentTarget;

            context.ActionPerformer.PerformAction(new Attack(target, owner));
        }
    }
}
