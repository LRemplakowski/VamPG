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

        public static void SaveObjects()
        {
            foreach (ISaveRuntimeData saveable in DataSet)
            {
                saveable.SaveRuntimeData();
            }
            ES3.Save(SCENE_INDEX_ID, SceneManager.GetActiveScene().buildIndex);
        }

        public static void LoadObjects()
        {
            foreach (ISaveRuntimeData saveable in DataSet)
            {
                saveable.LoadRuntimeData();
            }
        }

        public static int GetSavedSceneIndex()
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
