using System;
using Sirenix.OdinInspector;
using SunsetSystems.Core.SceneLoading;
using SunsetSystems.Input.CameraControl;
using SunsetSystems.Persistence;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Game
{
    public class GameManager : SerializedMonoBehaviour, ISaveable
    {
        public static GameManager Instance { get; private set; }

        public static event Action<GameState> OnGameStateChanged;

        [field: Title("References")]
        [field: SerializeField]
        public CameraControlScript GameCamera { get; private set; }

        [Title("Runtime")]
        [SerializeField]
        private GameState _gameState;
        public GameState CurrentState
        {
            get
            {
                return _gameState;
            }
            set
            {
                _gameState = value;
                OnGameStateChanged?.Invoke(value);
            }
        }

        [Title("Events")]
        // Called when level is done loading but before injecting persistence data in ISaveable objects
        public UltEvent OnBeforePersistentDataLoad = new();
        // Called when level is done loading & all the persistent data already is injected
        public UltEvent OnLevelStart = new();
        // Called before unloading current level and before caching the persistent data in ISaveable objects
        public UltEvent OnBeforePersistentDataCache = new();
        // Called when we start to unload the current level and all the persistent data has been cached
        public UltEvent OnLevelExit = new();

        public string DataKey => DataKeyConstants.GAME_MANAGER_DATA_KEY;

        protected void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
            ISaveable.RegisterSaveable(this);
            LevelLoader.OnBeforePersistentDataCache += BeforePersistentDataCache;
            LevelLoader.OnLevelLoadEnd += GameLevelStart;
            LevelLoader.OnLevelLoadStart += GameLevelEnd;
            LevelLoader.OnBeforePersistentDataLoad += BeforePersistentDataLoad;
        }

//        private void Start()
//        {
//#if UNITY_EDITOR
//            BeforePersistentDataLoad();
//            GameLevelStart();
//#endif
//        }

        private void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
            LevelLoader.OnBeforePersistentDataCache -= BeforePersistentDataCache;
            LevelLoader.OnLevelLoadEnd -= GameLevelStart;
            LevelLoader.OnLevelLoadStart -= GameLevelEnd;
            LevelLoader.OnBeforePersistentDataLoad -= BeforePersistentDataLoad;
        }

        private void BeforePersistentDataCache()
        {
            OnBeforePersistentDataCache?.InvokeSafe();
        }

        private void BeforePersistentDataLoad()
        {
            OnBeforePersistentDataLoad?.InvokeSafe();
        }

        private void GameLevelStart()
        {
            OnLevelStart?.InvokeSafe();
        }

        private void GameLevelEnd()
        {
            OnLevelExit?.InvokeSafe();
        }

        public string GetLanguage()
        {
            return "EN";
        }

        public bool IsCurrentState(GameState state)
        {
            return CurrentState.Equals(state);
        }

        public object GetSaveData()
        {
            GameManagerSaveData saveData = new()
            {
                CurrentState = CurrentState
            };
            return saveData;
        }

        public bool InjectSaveData(object data)
        {
            if (data is not GameManagerSaveData savedData)
                return false;
            CurrentState = savedData.CurrentState;
            return true;
        }

        private class GameManagerSaveData : SaveData
        {
            public GameState CurrentState;
        }
    }

    public enum GameState
    {
        Exploration,
        Combat,
        Dialogue,
        MainMenu,
        GamePaused, 
        WorldMap
    }
}
