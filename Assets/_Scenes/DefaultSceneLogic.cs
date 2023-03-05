using SunsetSystems.Data;
using UnityEngine;
using System.Threading.Tasks;
using SunsetSystems.Game;

namespace SunsetSystems.LevelManagement
{
    public class DefaultSceneLogic : AbstractSceneLogic
    {
        [SerializeField]
        private Waypoint _defaultEntryWaypoint;

        public async override Task StartSceneAsync(LevelLoadingData data)
        {
            Debug.Log("Starting scene");
            GameManager.CurrentState = GameState.Exploration;
            await Task.Yield();
        }

        public sealed override void InjectSaveData(object data)
        {
            
        }

        public sealed override object GetSaveData()
        {
            return new SceneLogicData();
        }
    }

    public class SceneLogicData : SaveData
    {

    }
}
