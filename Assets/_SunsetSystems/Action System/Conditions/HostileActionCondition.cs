using Sirenix.Serialization;
using SunsetSystems.Combat;
using UnityEngine;

namespace SunsetSystems.ActionSystem
{
    [System.Serializable]
    public class HostileActionCondition : Condition
    {
        [field: SerializeField]
        public bool Performed { get; set; }
        [OdinSerialize]
        private ICombatant target, performer;

        public HostileActionCondition(ICombatant target, ICombatant performer)
        {
            Performed = false;
            this.target = target;
            this.performer = performer;
        }

        public void OnHostileActionFinished(ICombatant target, ICombatant performer)
        {
            if ((target?.Equals(this.target) ?? false) && (performer?.Equals(this.performer) ?? false))
                Finish();
        }

        public void Finish()
        {
            Performed = true;
        }

        public override bool IsMet()
        {
            return Performed;
        }

        public override string ToString()
        {
            string result = "Type<HostileAction>:\n"
                + "Performed? " + Performed;
            return result;
        }
    } 
}