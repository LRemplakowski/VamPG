using CleverCrow.Fluid.UniqueIds;
using Glitchers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.SaveLoad
{
    public static class SaveLoadManager
    {
        public static void Save()
        {

            List<ISaveable> saveables = FindInterfaces.Find<ISaveable>();
            foreach (ISaveable saveable in saveables)
            {
                ES3.Save(saveable.GetData().Key, saveable.GetData());
            }    
        }

        public static void Load()
        {
            List<ISaveable> saveables = FindInterfaces.Find<ISaveable>();
            foreach (ISaveable saveable in saveables)
            {
                saveable.InjectData(ES3.Load<SerializedData>(saveable.GetData().Key));
            }
        }
    }

    public interface ISaveable
    {
        SerializedData GetData();

        void InjectData(SerializedData data);
    }

    [System.Serializable]
    public abstract class SerializedData
    {
        public abstract string Key { get; }
    }
}
