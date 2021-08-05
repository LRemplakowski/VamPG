namespace AI.Actions
{
    using Apex.AI;
    using System.Collections.Generic;

    public class AttackCurrentTarget : ActionBase<CreatureContext>
    {
        public override void Execute(CreatureContext context)
        {
            Creature owner = context.Owner;
            Creature target = context.CurrentTarget;

            owner.Attack(target);
        }
    }
}
