using System.Collections.Generic;

namespace SunsetSystems.Persistence
{
    public interface IPersistentObject
    {
        object GetPersistenceData();

        void InjectPersistenceData(object data);

        string PersistenceID { get; }

        string GameObjectName { get; }

        bool EnablePersistence { get; }

        List<IPersistentComponent> PersistentComponents { get; }
    }
}
