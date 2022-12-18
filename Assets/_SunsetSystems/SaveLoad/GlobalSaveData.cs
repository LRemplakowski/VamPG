using System;
using System.Collections.Generic;

namespace SunsetSystems.Loading
{
    [Serializable]
    public class GlobalSaveData
    {
        [ES3Serializable]
        private Dictionary<string, object> _saveDataDictionary = new();

        public void UpdateSaveData<T>(T dataProvider) where T : ISaveable
        {
            if (_saveDataDictionary.TryAdd(dataProvider.DataKey, dataProvider.GetSaveData()))
                _saveDataDictionary[dataProvider.DataKey] = dataProvider.GetSaveData();
        }

        public object GetData(string dataKey)
        {
            if (_saveDataDictionary.TryGetValue(dataKey, out object data))
                return data;
            return null;
        }

        public void ClearSaveData()
        {
            _saveDataDictionary.Clear();
        }
    }

    [Serializable]
    public abstract class SaveData
    {

    }

    public interface ISaveable
    {
        string DataKey { get; }
        object GetSaveData();
        void InjectSaveData(object data);
    }
}
