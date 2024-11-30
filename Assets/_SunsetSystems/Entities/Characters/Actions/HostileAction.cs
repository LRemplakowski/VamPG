using System;
using Sirenix.Serialization;
using SunsetSystems.Combat;

namespace SunsetSystems.Entities.Characters.Actions
{
    [Serializable]
    public abstract class HostileAction : EntityAction
    {
        [field: OdinSerialize]
        protected ICombatant Attacker { get; private set; }
        [field: OdinSerialize]
        protected ITargetable Target { get; private set; }

        public HostileAction(ITargetable target, ICombatant attacker) : base(attacker, false)
        {
            Attacker = attacker;
            Target = target;
        }
    }
}