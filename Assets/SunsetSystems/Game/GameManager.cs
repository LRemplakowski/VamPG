using SunsetSystems.Utils;
using UnityEngine;
using System;

namespace SunsetSystems.Game
{
    [RequireComponent(typeof(Tagger))]
    public class GameManager : Singleton<GameManager>
    {
        public static event Action<GameState> OnGameStateChanged;

        private static GameState _gameState;
        public static GameState CurrentState 
        { 
            get
            {
                return _gameState;
            }
            set
            {
                _gameState = value;
                OnGameStateChanged?.Invoke(_gameState);
            }
        }

        public static string GetLanguage()
        {
            return "EN";
        }

        public static bool IsCurrentState(GameState state)
        {
            return CurrentState.Equals(state);
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
