using Redcode.Awaiting;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace SunsetSystems.Loading
{
    public static class SaveLoadManager
    {
        private const string SCENE_INDEX_ID = "SceneIndex";
        private const string SAVE_DATA_KEY = "SAVE_DATA";

        public static readonly HashSet<ISaveable> TrackedSaveDataProviders = new();
        private static GlobalSaveData _saveData = new();

        public static void SaveObjects()
        {
            foreach (ISaveable saveable in TrackedSaveDataProviders)
            {
                _saveData.UpdateSaveData(saveable);
            }
            ES3.Save(SAVE_DATA_KEY, _saveData);
            ES3.Save(SCENE_INDEX_ID, SceneManager.GetActiveScene().buildIndex);
        }

        public static void LoadObjects()
        {
            _saveData = ES3.Load<GlobalSaveData>(SAVE_DATA_KEY);
            foreach (ISaveable saveable in TrackedSaveDataProviders)
            {
                saveable.InjectSaveData(_saveData.GetData(saveable.DataKey));
            }
        }

        public static int GetSavedSceneIndex()
        {
            return ES3.Load<int>(SCENE_INDEX_ID);
        }
    }
}
