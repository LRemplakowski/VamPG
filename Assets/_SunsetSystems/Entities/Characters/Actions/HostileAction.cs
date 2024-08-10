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
        protected ICombatant Target { get; private set; }

        public HostileAction(ICombatant target, ICombatant attacker) : base(attacker, false)
        {
            Attacker = attacker;
            Target = target;
        }
    }
}