using System;
using System.Threading.Tasks;
using SunsetSystems.Game;
using SunsetSystems.Persistence;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Core
{
    public class DefaultSceneLogic : AbstractSceneLogic
    {
        [field: SerializeField]
        public UltEvent OnSceneStart { get; private set; }

        public async override Task StartSceneAsync()
        {
            OnSceneStart?.InvokeSafe();
            await Task.Yield();
        }

        public override void InjectSaveData(object data)
        {
            
        }

        public override object GetSaveData()
        {
            SceneLogicData saveData = new();
            return saveData;
        }

        [Serializable]
        protected class SceneLogicData : SaveData
        {

        }
    }
}
