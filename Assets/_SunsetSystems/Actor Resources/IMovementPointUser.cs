namespace SunsetSystems.ActorResources
{
    public interface IMovementPointUser
    {
        int GetCurrentMovementPoints();

        bool GetCanMove();

        bool UseMovementPoints(int amonut);

        void AddMovementPointUseBlocker(string sourceID);
        void RemoveMovementPointUseBlocker(string sourceID);
    }
}
