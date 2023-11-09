using SunsetSystems.Entities.Interfaces;

namespace SunsetSystems.Entities.Characters.Actions.Conditions
{
    [System.Serializable]
    public class HostileActionCondition : Condition
    {
        public bool Performed { get; set; }

        private readonly ICombatant target, performer;

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