using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Interfaces;
using System;

namespace SunsetSystems.Entities.Characters.Actions
{
    public abstract class HostileAction : EntityAction
    {
        protected new ICombatant Owner { get; }
        protected ICombatant Target { get; private set; }

        public HostileAction(ICombatant target, ICombatant attacker) : base(attacker, true)
        {
            Owner = attacker;
            Target = target;
        }
    }
}