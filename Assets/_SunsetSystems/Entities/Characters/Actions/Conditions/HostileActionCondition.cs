using SunsetSystems.Entities.Characters.Interfaces;

namespace SunsetSystems.Entities.Characters.Actions.Conditions
{
    public class HostileActionCondition : Condition
    {
        public bool Performed { get; set; }

        private readonly ICreature target, performer;

        public HostileActionCondition(ICreature target, ICreature performer)
        {
            Performed = false;
            this.target = target;
            this.performer = performer;
        }

        public void OnHostileActionFinished(ICreature target, ICreature performer)
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