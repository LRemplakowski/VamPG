using SunsetSystems.Utils;
using UnityEngine;
using System;

namespace SunsetSystems.Game
{
    [RequireComponent(typeof(Tagger))]
    public class GameManager : Singleton<GameManager>
    {
        public static event Action<GameState> OnGameStateChanged;

        public static GameState CurrentState 
        { 
            get
            {
                return CurrentState;
            }
            set
            {
                CurrentState = value;
                OnGameStateChanged?.Invoke(value);
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
