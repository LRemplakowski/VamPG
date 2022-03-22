using Glitchers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transitions.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using SunsetSystems.Scenes;

namespace SunsetSystems.SaveLoad
{
    public static class SaveLoadManager
    {
        private const string SCENE_INDEX_ID = "SceneIndex";

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

        internal static async Task LoadObjects()
        {
            List<ISaveRuntimeData> saveables = FindInterfaces.Find<ISaveRuntimeData>();
            Debug.Log("Loadables found: " + saveables.Count);
            foreach (ISaveRuntimeData saveable in saveables)
            {
                saveable.LoadRuntimeData();
                await Task.Yield();
            }
        }

        internal static int GetSavedSceneIndex()
        {
            return ES3.Load<int>(SCENE_INDEX_ID);
        }
    }

    public interface ISaveRuntimeData
    {
        void SaveRuntimeData();

        void LoadRuntimeData();
    }
}
