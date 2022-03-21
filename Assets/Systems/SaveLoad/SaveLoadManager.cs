using Glitchers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transitions.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Scenes;

namespace SunsetSystems.SaveLoad
{
    public static class SaveLoadManager
    {
        private const string SCENE_INDEX_ID = "SceneIndex";
        public static event Action OnRuntimeDataLoaded;

        public static void Save()
        {
            List<ISaveRuntimeData> saveables = FindInterfaces.Find<ISaveRuntimeData>();
            Debug.Log("Saveables found: " + saveables.Count);
            foreach (ISaveRuntimeData saveable in saveables)
            {
                saveable.SaveRuntimeData();
            }
            ES3.Save(SCENE_INDEX_ID, SceneManager.GetActiveScene().buildIndex);
        }

        public static async Task Load()
        {
            int sceneIndex = ES3.Load<int>(SCENE_INDEX_ID);
            await UnityEngine.Object.FindObjectOfType<SceneLoader>().LoadSavedScene(new IndexLoadingData(sceneIndex, ""));
            await LoadObjects();
        }

        private static async Task LoadObjects()
        {
            List<ISaveRuntimeData> saveables = FindInterfaces.Find<ISaveRuntimeData>();
            Debug.Log("Loadables found: " + saveables.Count);
            foreach (ISaveRuntimeData saveable in saveables)
            {
                saveable.LoadRuntimeData();
                await Task.Yield();
            }
            OnRuntimeDataLoaded?.Invoke();
        }
    }

    public interface ISaveRuntimeData
    {
        void SaveRuntimeData();

        void LoadRuntimeData();
    }
}
