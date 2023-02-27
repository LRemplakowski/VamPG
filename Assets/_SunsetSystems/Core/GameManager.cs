using SunsetSystems.Utils;
using UnityEngine;
using System;
using SunsetSystems.LevelManagement;
using CleverCrow.Fluid.UniqueIds;

namespace SunsetSystems.Game
{
    [RequireComponent(typeof(UniqueId))]
    [RequireComponent(typeof(Tagger))]
    public class GameManager : MonoBehaviour, IGameManager, ISaveable
    {
        public static event Action<GameState> OnGameStateChanged;

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

        public string DataKey => _uniqueId.Id;

        private UniqueId _uniqueId;

        protected void Awake()
        {
            _uniqueId ??= GetComponent<UniqueId>();
            ISaveable.RegisterSaveable(this);
        }

        private void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
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
            GameManagerSaveData saveData = new();
            saveData.CurrentState = CurrentState;
            return saveData;
        }

        public void InjectSaveData(object data)
        {
            GameManagerSaveData savedData = data as GameManagerSaveData;
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
