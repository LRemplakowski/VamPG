using UnityEngine;
using System.Threading.Tasks;
using UltEvents;
using SunsetSystems.LevelUtility;
using SunsetSystems.Game;
using SunsetSystems.Persistence;
using System;

namespace SunsetSystems.Core
{
    public class DefaultSceneLogic : AbstractSceneLogic
    {
        [SerializeField]
        private Waypoint _defaultEntryWaypoint;

        [field: SerializeField]
        public UltEvent OnSceneStart { get; private set; }

        public async override Task StartSceneAsync()
        {
            await Task.Yield();
            GameManager.Instance.GameCamera.ForceToPosition(_defaultEntryWaypoint.transform);
            OnSceneStart?.InvokeSafe();
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
