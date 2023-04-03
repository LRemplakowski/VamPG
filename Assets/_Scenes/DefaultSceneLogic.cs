using SunsetSystems.Data;
using UnityEngine;
using System.Threading.Tasks;
using SunsetSystems.Game;
using SunsetSystems.Party;

namespace SunsetSystems.Persistence
{
    public class DefaultSceneLogic : AbstractSceneLogic
    {
        [SerializeField]
        private Waypoint _defaultEntryWaypoint;

        public async override Task StartSceneAsync(LevelLoadingData data)
        {
            PartyManager.ActiveParty.ForEach(p => p.ForceCreatureToPosition(_defaultEntryWaypoint.transform.position));
            GameManager.CurrentState = GameState.Exploration;
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
