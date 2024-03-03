using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SunsetSystems.Persistence
{
    public static class SaveLoadManager
    {
        private const string SCENE_INDEX_ID = "SceneIndex";
        private const string SAVE_DATA_KEY = "SAVE_DATA";
        private const string LAST_SAVE_FILENAME = "LAST_SAVE_FILENAME";
        private const string SAVE_META_FILE_NAME = "SunsetSave.meta";
        private const string CURRENT_SAVE_ID = "SAVE_ID";

        private static GlobalPersistenceData _saveData = new();

        public static void ForceCreateNewSaveData()
        {
            _saveData = new();
        }

        public static void CreateNewSaveFileAndSaveObjects()
        {
            if (_saveData == null)
                _saveData = new();
            string filename = $"{DateTime.Now:yyyy-M-dd--HH-mm-ss}.sav";
            ES3.Save(LAST_SAVE_FILENAME, filename, SAVE_META_FILE_NAME);
            UpdateRuntimeDataCache();
            try
            {
                ES3.Save(SAVE_DATA_KEY, _saveData, filename);
            }
            catch (NotSupportedException exception)
            {
                Debug.LogException(exception);
            }
            ES3.Save(SCENE_INDEX_ID, SceneManager.GetActiveScene().buildIndex, filename);
            ES3.Save(CURRENT_SAVE_ID, ES3.Load<string>(CURRENT_SAVE_ID, SAVE_META_FILE_NAME), filename);
        }

        public static void UpdateRuntimeDataCache()
        {
            if (_saveData == null)
                _saveData = new();
            foreach (ISaveable saveable in ISaveable.Saveables)
            {
                _saveData.UpdateSaveData(saveable);
            }
        }

        public static void LoadSavedDataIntoRuntime()
        {
            string saveID = ES3.Load<string>(CURRENT_SAVE_ID, SAVE_META_FILE_NAME);
            string path = ES3.Load<string>(LAST_SAVE_FILENAME, SAVE_META_FILE_NAME);
            if (string.IsNullOrWhiteSpace(path))
                return;
            if (string.Equals(saveID, ES3.Load<string>(CURRENT_SAVE_ID, path)) is false)
                return;
            if (_saveData == null)
                _saveData = new();
            _saveData.ClearSaveData();
            ES3.LoadInto(SAVE_DATA_KEY, path, _saveData);
        }

        public static void InjectRuntimeDataIntoSaveables()
        {
            if (_saveData == null)
                throw new NullReferenceException("GlobalSaveData instance is null!");
            foreach (ISaveable saveable in ISaveable.Saveables)
            {
                saveable.InjectSaveData(_saveData.GetData(saveable.DataKey));
            }
        }

        public static void SetSaveID(Guid guid)
        {
            ES3.Save(CURRENT_SAVE_ID, guid.ToString(), SAVE_META_FILE_NAME);
        }

        public static int GetSavedSceneIndex()
        {
            string path = ES3.Load<string>(LAST_SAVE_FILENAME, SAVE_META_FILE_NAME);
            return ES3.Load<int>(SCENE_INDEX_ID, path);
        }
    }
}
