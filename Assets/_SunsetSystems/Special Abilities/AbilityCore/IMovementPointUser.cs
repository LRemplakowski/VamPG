namespace SunsetSystems.Abilities
{
    public interface IMovementPointUser
    {
        int GetCurrentMovementPoints();
        bool GetCanMove();

        bool UseMovementPoints(int amonut);
    }
}
