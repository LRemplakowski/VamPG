using System;
using UnityEngine;

namespace SunsetSystems.Game
{
    internal class StateManager
    {
        [SerializeField]
        private GameState _currentState = GameState.Menu;

        public GameState CurrentState { get => _currentState; set => _currentState = value; }
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
