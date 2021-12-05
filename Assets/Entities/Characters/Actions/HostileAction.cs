using Entities.Characters.Actions.Conditions;

namespace Entities.Characters.Actions
{
    public abstract class HostileAction : EntityAction
    {
        protected override Creature Owner { get; set; }

        public delegate void OnAttackFinished(Creature target, Creature performer);
        public static OnAttackFinished onAttackFinished;

        public Creature Target { get; private set; }
        protected readonly HostileActionCondition condition;

        public HostileAction(Creature target, Creature attacker)
        {
            Owner = attacker;
            Target = target;
            HostileActionCondition condition = new HostileActionCondition(target, attacker);
            onAttackFinished += condition.OnHostileActionFinished;
            conditions.Add(condition);
            this.condition = condition;
        }

        public override void Abort()
        {
            onAttackFinished -= condition.OnHostileActionFinished;
        }
    } 
}