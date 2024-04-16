using System;
using System.Collections.Generic;
using System.Diagnostics;
using SunsetSystems.Core.SceneLoading;
using UnityEngine.SceneManagement;

namespace SunsetSystems.Persistence
{
    public static class SaveLoadManager
    {
        private const string META_DATA = "SAVE_META";
        private const string GAME_DATA = "SAVE_GAME";

        private static GlobalPersistenceData _gameData = new();

        public static void ForceCreateNewSaveData()
        {
            _gameData = new();
        }

        public static void CreateNewSaveFile(string saveName)
        {
            string date = $"{DateTime.Now:yyyy-M-dd--HH-mm-ss}";
            string saveID = Guid.NewGuid().ToString();
            string filename = SaveIDToFileName(saveID);
            SaveMetaData metaData = new()
            {
                SaveName = saveName,
                SaveID = saveID,
                SaveDate = date,
                LevelLoadingData = LevelLoader.Instance.CurrentLoadedLevel,
            };
            UpdateRuntimeDataCache();
            ES3.Save(META_DATA, metaData, filename);
            ES3.Save(GAME_DATA, _gameData, filename);
        }

        public static void UpdateRuntimeDataCache()
        {
            foreach (ISaveable saveable in ISaveable.Saveables)
            {
                _gameData.UpdateSaveData(saveable);
            }
        }

        public static void LoadSavedDataIntoRuntime(string saveID)
        {
            //_gameData.ClearSaveData();
            ES3.LoadInto(GAME_DATA, SaveIDToFileName(saveID), _gameData);
        }

        public static IEnumerable<SaveMetaData> GetAllSaveMetaData()
        {
            var saveFiles = ES3.GetFiles();
            List<SaveMetaData> result = new();
            foreach (var saveFile in saveFiles)
            {
                result.Add(ES3.Load<SaveMetaData>(META_DATA, saveFile));
            }
            return result;
        }

        public static SaveMetaData GetSaveMetaData(string saveID)
        {
            var metaData = ES3.Load<SaveMetaData>(META_DATA, SaveIDToFileName(saveID));
            return metaData;
        }

        public static void InjectRuntimeDataIntoSaveables()
        {
            foreach (ISaveable saveable in ISaveable.Saveables)
            {
                if (_gameData.TryGetData(saveable.DataKey, out object data))
                    saveable.InjectSaveData(data);
                else
                    UnityEngine.Debug.Log($"There is no saved data for object {saveable}!");
            }
        }

        public static SceneLoadingDataAsset.LevelLoadingData GetSavedLevelAsset(string saveID)
        {
            var metaData = ES3.Load<SaveMetaData>(META_DATA, SaveIDToFileName(saveID));
            if (metaData.SaveID == saveID)
                return metaData.LevelLoadingData;
            return new();
        }

        public static void DeleteSaveFile(string saveID)
        {
            string saveFileName = SaveIDToFileName(saveID);
            ES3.DeleteFile(saveFileName);
        }

        private static string SaveIDToFileName(string saveID)
        {
            return $"{saveID}.sav";
        }

        private static string SaveFileNameToSaveID(string fileName)
        {
            return fileName.Split('.')[0];
        }

        public static bool HasExistingSaves()
        {
            return ES3.GetFiles().Length > 0;
        }
    }
}
