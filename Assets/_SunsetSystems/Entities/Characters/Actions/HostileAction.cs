using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Interfaces;
using System;

namespace SunsetSystems.Entities.Characters.Actions
{
    [System.Serializable]
    public abstract class HostileAction : EntityAction
    {
        [ShowInInspector]
        protected new ICombatant Owner { get; }
        [ShowInInspector]
        protected ICombatant Target { get; }

        public HostileAction(ICombatant target, ICombatant attacker) : base(attacker, true)
        {
            Owner = attacker;
            Target = target;
        }
    }
}