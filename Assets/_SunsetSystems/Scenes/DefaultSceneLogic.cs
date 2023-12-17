using SunsetSystems.Data;
using UnityEngine;
using System.Threading.Tasks;
using UltEvents;
using SunsetSystems.LevelUtility;

namespace SunsetSystems.Persistence
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
        }

        public override void InjectSaveData(object data)
        {
            
        }

        public override object GetSaveData()
        {
            return null;
        }

        protected class SceneLogicData : SaveData
        {

        }
    }
}
