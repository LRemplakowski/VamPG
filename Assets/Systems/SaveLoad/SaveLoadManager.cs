using Glitchers;
using System.Collections.Generic;
using Transitions.Data;
using Transitions.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SunsetSystems.SaveLoad
{
    public static class SaveLoadManager
    {
        private const string SCENE_INDEX_ID = "SceneIndex";

        public static void Save()
        {
            List<ISaveRuntimeData> saveables = FindInterfaces.Find<ISaveRuntimeData>();
            foreach (ISaveRuntimeData saveable in saveables)
            {
                saveable.SaveRuntimeData();
            }
            ES3.Save(SCENE_INDEX_ID, SceneManager.GetActiveScene().buildIndex);
        }

        public static void Load()
        {
            int sceneIndex = ES3.Load<int>(SCENE_INDEX_ID);
            Object.FindObjectOfType<TransitionManager>().PerformTransition(new IndexTransition(sceneIndex, ""));
            SceneManager.sceneLoaded += LoadObjects;
        }

        private static void LoadObjects(Scene scene, LoadSceneMode loadSceneMode)
        {
            List<ISaveRuntimeData> saveables = FindInterfaces.Find<ISaveRuntimeData>();
            foreach (ISaveRuntimeData saveable in saveables)
            {
                saveable.LoadRuntimeData();
            }
            SceneManager.sceneLoaded -= LoadObjects;
        }
    }

    public interface ISaveRuntimeData
    {
        void SaveRuntimeData();

        void LoadRuntimeData();
    }
}
