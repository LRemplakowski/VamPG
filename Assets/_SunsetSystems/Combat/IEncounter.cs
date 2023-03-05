namespace SunsetSystems.Combat
{
    public interface IEncounter
    {
        GridController MyGrid { get; }

        void Begin();
        void End();
    }
}
