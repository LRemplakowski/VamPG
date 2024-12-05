namespace SunsetSystems.Abilities
{
    public interface IMovementPointUser
    {
        int GetCurrentMovementPoints();

        bool UseMovementPoints(int amonut);
    }
}
