using CleverCrow.Fluid.UniqueIds;
using Glitchers;
using SunsetSystems.Journal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Transitions.Data;
using Transitions.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Singleton;

namespace SunsetSystems.SaveLoad
{
    public static class SaveLoadManager
    {
        [SerializeField]
        private static List<string> keys = new List<string>();

        public static void Save()
        {
            keys.Clear();
            List<UniqueId> uniques = new List<UniqueId>(UnityEngine.Object.FindObjectsOfType<UniqueId>());
            foreach (UniqueId unique in uniques)
            {
                Debug.Log("saving gameobject " + unique.Id);
                ES3.Save(unique.Id, unique.gameObject);
                ISaveRuntimeData[] saveRuntimeData = unique.GetComponentsInChildren<ISaveRuntimeData>();
                foreach (ISaveRuntimeData save in saveRuntimeData)
                    save.SaveRuntimeData();
                keys.Add(unique.Id);
            }
            ES3.Save("KeyList", keys);
            ES3.Save("SceneIndex", SceneManager.GetActiveScene().buildIndex);
        }

        public static void Load()
        {
            int sceneIndex = ES3.Load<int>("SceneIndex");
            TransitionManager.Instance.PerformTransition(new IndexTransition(sceneIndex, "", LoadSceneMode.Single));
            SceneManager.sceneLoaded += LoadObjects;
        }

        private static void LoadObjects(Scene scene, LoadSceneMode loadSceneMode)
        {
            List<string> keys = ES3.Load<List<string>>("KeyList");
            foreach (string key in keys)
            {
                GameObject obj = ES3.Load<GameObject>(key);
                
                ISaveRuntimeData[] loadRuntimeData = obj.GetComponentsInChildren<ISaveRuntimeData>();
                foreach (ISaveRuntimeData load in loadRuntimeData)
                    load.LoadRuntimeData();
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
