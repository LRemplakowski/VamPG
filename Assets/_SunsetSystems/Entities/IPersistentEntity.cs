namespace SunsetSystems.Entities.Interfaces
{
    public interface IPersistentEntity : IEntity
    {
        object GetPersistenceData();

        void InjectPersistenceData(object data);

        string PersistenceID { get; }

        string GameObjectName { get; }
    }
}
