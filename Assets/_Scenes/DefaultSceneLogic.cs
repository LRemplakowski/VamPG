using SunsetSystems.Data;
using UnityEngine;
using System.Threading.Tasks;
using SunsetSystems.Game;
using Zenject;

namespace SunsetSystems.LevelManagement
{
    public class DefaultSceneLogic : AbstractSceneLogic
    {
        [SerializeField]
        private Waypoint _defaultEntryWaypoint;

        protected IGameManager _gameManager;

        [Inject]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Dependency Injection")]
        private void InjectDependencies(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public async override Task StartSceneAsync(LevelLoadingData data)
        {
            Debug.Log("Starting scene");
            _gameManager.CurrentState = GameState.Exploration;
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
