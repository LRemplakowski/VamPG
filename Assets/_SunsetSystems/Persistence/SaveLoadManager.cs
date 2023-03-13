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

        private static GlobalSaveData _saveData = new();

        public static void SaveObjects()
        {
            string filename = $"{DateTime.Now:yyyy-M-dd--HH-mm-ss}.sav";
            ES3.Save(LAST_SAVE_FILENAME, filename, SAVE_META_FILE_NAME);
            foreach (ISaveable saveable in ISaveable.Saveables)
            {
                _saveData.UpdateSaveData(saveable);
            }
            try
            {
                ES3.Save(SAVE_DATA_KEY, _saveData, filename);
            }
            catch (NotSupportedException exception)
            {
                Debug.LogException(exception);
            }
            ES3.Save(SCENE_INDEX_ID, SceneManager.GetActiveScene().buildIndex, filename);
        }

        public static void LoadObjects()
        {
            string path = ES3.Load<string>(LAST_SAVE_FILENAME, SAVE_META_FILE_NAME);
            _saveData = ES3.Load<GlobalSaveData>(SAVE_DATA_KEY, path);
            foreach (ISaveable saveable in ISaveable.Saveables)
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
