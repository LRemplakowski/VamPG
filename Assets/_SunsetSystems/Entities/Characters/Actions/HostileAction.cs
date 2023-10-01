using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Interfaces;
using System;

namespace SunsetSystems.Entities.Characters.Actions
{
    public abstract class HostileAction : EntityAction
    {
        public static event Action<ICombatant, ICombatant> OnAttackFinished;
        protected new ICombatant Owner { get; }
        protected ICombatant Target { get; private set; }
        protected readonly HostileActionCondition condition;

        public HostileAction(ICombatant target, ICombatant attacker) : base(attacker, true)
        {
            Owner = attacker;
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

        protected void Finish(ICombatant target, ICombatant attacker) => OnAttackFinished?.Invoke(target, attacker);
    }
}