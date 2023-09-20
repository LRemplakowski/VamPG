using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Interfaces;
using System;

namespace SunsetSystems.Entities.Characters.Actions
{
    public abstract class HostileAction : EntityAction
    {
        public static event Action<ICombatant, ICombatant> OnAttackFinished;


        public ICombatant Target { get; private set; }
        protected readonly HostileActionCondition condition;

        public HostileAction(ICombatant target, ICombatant attacker) : base(attacker.References.GetComponent<Creature>(), true)
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

        protected void Finish(ICombatant target, ICombatant attacker) => OnAttackFinished?.Invoke(target, attacker);
    }
}