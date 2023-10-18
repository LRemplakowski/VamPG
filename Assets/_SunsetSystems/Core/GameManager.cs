using SunsetSystems.Utils;
using UnityEngine;
using System;
using SunsetSystems.Persistence;
using CleverCrow.Fluid.UniqueIds;
using Sirenix.OdinInspector;

namespace SunsetSystems.Game
{
    [RequireComponent(typeof(UniqueId))]
    [RequireComponent(typeof(Tagger))]
    public class GameManager : SerializedMonoBehaviour, ISaveable
    {
        public static GameManager Instance { get; private set; }

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
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
            if (_uniqueId == null)
                _uniqueId = GetComponent<UniqueId>();
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
            GameManagerSaveData saveData = new()
            {
                CurrentState = CurrentState
            };
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
