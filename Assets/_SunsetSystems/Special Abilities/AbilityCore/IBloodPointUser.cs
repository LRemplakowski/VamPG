namespace SunsetSystems.Abilities
{
    public interface IBloodPointUser
    {
        int GetCurrentBloodPoints();

        bool UseBloodPoints(int amount);
    }
}
