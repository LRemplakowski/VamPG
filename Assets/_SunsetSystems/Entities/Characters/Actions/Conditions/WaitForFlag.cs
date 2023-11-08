namespace SunsetSystems.Entities.Characters.Actions.Conditions
{
    public class WaitForFlag : Condition
    {
        private readonly bool flag;

        public WaitForFlag(ref bool flag)
        {
            this.flag = flag;
        }

        public override bool IsMet()
        {
            return flag;
        }
    }
}
