using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace SunsetSystems.Persistence
{
    [Serializable]
    public class GlobalPersistenceData
    {
        [ShowInInspector]
        private Dictionary<string, object> _persistenceDataDictionary = new();

        public void UpdateSaveData<T>(T dataProvider) where T : ISaveable
        {
            if (dataProvider is null || string.IsNullOrWhiteSpace(dataProvider.DataKey))
                return;
            object saveData = dataProvider.GetSaveData();
            if (saveData is null)
                return;
            if (_persistenceDataDictionary.TryAdd(dataProvider.DataKey, saveData) is false)
                _persistenceDataDictionary[dataProvider.DataKey] = saveData;
        }

        public bool TryGetData(string dataKey, out object data)
        {
            data = null;
            if (dataKey != null && _persistenceDataDictionary.TryGetValue(dataKey, out data))
                return true;
            return false;
        }

        public void ClearSaveData()
        {
            _persistenceDataDictionary.Clear();
        }
    }

    [Serializable]
    public abstract class SaveData
    {

    }

    public interface ISaveable
    {
        private static readonly HashSet<ISaveable> SaveablesCache = new();
        public static IEnumerable<ISaveable> Saveables => SaveablesCache.AsEnumerable();

        protected static void RegisterSaveable(ISaveable saveable)
        {
            SaveablesCache.Add(saveable);
        }

        protected static void UnregisterSaveable(ISaveable saveable)
        {
            SaveablesCache.Remove(saveable);
        }

        string DataKey { get; }
        object GetSaveData();
        bool InjectSaveData(object data);
    }
}
