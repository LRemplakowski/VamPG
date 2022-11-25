namespace SunsetSystems.Entities.Characters.Actions.Conditions
{
    public class HostileActionCondition : Condition
    {

        public bool Performed { get; set; }

        private Creature target, performer;

        public HostileActionCondition(Creature target, Creature performer)
        {
            Performed = false;
            this.target = target;
            this.performer = performer;
        }

        public void OnHostileActionFinished(Creature target, Creature performer)
        {
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