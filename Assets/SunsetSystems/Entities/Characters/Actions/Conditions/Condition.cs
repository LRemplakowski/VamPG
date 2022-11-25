namespace SunsetSystems.Entities.Characters.Actions.Conditions
{
    public abstract class Condition
    {
        public abstract bool IsMet();

        public abstract override string ToString();
    }
}