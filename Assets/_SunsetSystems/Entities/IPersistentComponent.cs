namespace SunsetSystems.Persistence
{
    public interface IPersistentComponent
    {
        string ComponentID { get; }

        object GetComponentPersistenceData();
        void InjectComponentPersistenceData(object data);
    }
}
