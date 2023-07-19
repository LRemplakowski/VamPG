using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Entities.Characters.Interfaces;
using System;

namespace SunsetSystems.Entities.Characters.Actions
{
    public abstract class HostileAction : EntityAction
    {
        public static event Action<ICreature, ICreature> OnAttackFinished;

        public ICreature Target { get; private set; }
        protected readonly HostileActionCondition condition;

        public HostileAction(ICreature target, ICreature attacker) : base(attacker, true)
        {
            Target = target;
            HostileActionCondition condition = new(target, attacker);
            OnAttackFinished += condition.OnHostileActionFinished;
            conditions.Add(condition);
            this.condition = condition;
        }

        public override void Abort()
        {
            OnAttackFinished -= condition.OnHostileActionFinished;
        }

        protected void Finish(ICreature target, ICreature attacker) => OnAttackFinished?.Invoke(target, attacker);
    }
}