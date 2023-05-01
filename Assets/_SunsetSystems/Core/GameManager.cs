using SunsetSystems.Utils;
using UnityEngine;
using System;
using SunsetSystems.Persistence;
using CleverCrow.Fluid.UniqueIds;

namespace SunsetSystems.Game
{
    [RequireComponent(typeof(UniqueId))]
    [RequireComponent(typeof(Tagger))]
    public class GameManager : Singleton<GameManager>, ISaveable
    {
        public static event Action<GameState> OnGameStateChanged;

        [SerializeField]
        private GameState _gameState;
        public static GameState CurrentState 
        { 
            get
            {
                return Instance._gameState;
            }
            set
            {
                Instance._gameState = value;
                OnGameStateChanged?.Invoke(value);
            }
        }

        public string DataKey => _uniqueId.Id;

        private UniqueId _uniqueId;

        protected override void Awake()
        {
            _uniqueId ??= GetComponent<UniqueId>();
            ISaveable.RegisterSaveable(this);
        }

        private void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
        }

        public static string GetLanguage()
        {
            return "EN";
        }

        public static bool IsCurrentState(GameState state)
        {
            return CurrentState.Equals(state);
        }

        public object GetSaveData()
        {
            GameManagerSaveData saveData = new GameManagerSaveData();
            saveData.CurrentState = CurrentState;
            return saveData;
        }

        public void InjectSaveData(object data)
        {
            if (data is not GameManagerSaveData savedData)
                return;
            CurrentState = savedData.CurrentState;
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
        Conversation,
        Menu,
        GamePaused
    }
}
