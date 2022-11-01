namespace SunsetSystems.Entities.Characters.Actions
{
    public class Idle : EntityAction
    {
        protected override Creature Owner
        {
            get;
            set;
        }

        public Idle(Creature owner)
        {
            Owner = owner;
        }

        public override void Abort()
        {

        }

        public override void Begin()
        {

        }

        public override bool IsFinished()
        {
            return false;
        }
    } 
}
