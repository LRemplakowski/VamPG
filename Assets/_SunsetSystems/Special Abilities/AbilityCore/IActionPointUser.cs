namespace SunsetSystems.Abilities
{
    public interface IActionPointUser
    {
        int GetCurrentActionPoints();

        bool UseActionPoints(int amount);
    }
}
