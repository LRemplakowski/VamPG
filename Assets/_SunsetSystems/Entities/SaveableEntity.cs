using SunsetSystems.Loading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities
{
    public abstract class SaveableEntity : Entity, ISaveRuntimeData
    {
        public abstract void LoadRuntimeData();
        public abstract void SaveRuntimeData();

        protected virtual void Awake()
        {
            SaveLoadManager.DataSet.Add(this);
            LoadRuntimeData();
        }

        private void OnDestroy()
        {
            SaveRuntimeData();
            SaveLoadManager.DataSet.Remove(this);
        }
    }
}
