using Glitchers;
using Redcode.Awaiting;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SunsetSystems.Loading
{
    public static class SaveLoadManager
    {
        private const string SCENE_INDEX_ID = "SceneIndex";

        public static readonly HashSet<ISaveRuntimeData> DataSet = new();

        public static async void Save()
        {
            List<ISaveRuntimeData> saveables = FindInterfaces.Find<ISaveRuntimeData>();
            Debug.Log("Saveables found: " + saveables.Count);
            await new WaitForBackgroundThread();
            foreach (ISaveRuntimeData saveable in saveables)
            {
                saveable.SaveRuntimeData();
            }
            await new WaitForUpdate();
            ES3.Save(SCENE_INDEX_ID, SceneManager.GetActiveScene().buildIndex);
        }

        internal static async Task LoadObjects()
        {
            List<ISaveRuntimeData> saveables = FindInterfaces.Find<ISaveRuntimeData>();
            Debug.Log("Loadables found: " + saveables.Count);
            await new WaitForBackgroundThread();
            foreach (ISaveRuntimeData saveable in saveables)
            {
                saveable.LoadRuntimeData();
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

        void Start()
        {
            Debug.Log("dupa");
            SaveLoadManager.DataSet.Add(this);
        }

        void OnDestroy()
        {
            SaveLoadManager.DataSet.Remove(this);
        }
    }
}
