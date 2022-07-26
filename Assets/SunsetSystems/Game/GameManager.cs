using Entities.Characters;
using SunsetSystems.Data;
using SunsetSystems.Utils;

namespace SunsetSystems.Game
{
    public class GameManager : Singleton<GameManager>
    {
        private static Creature _player;
        private static GridController _gridController;
        private readonly StateManager stateManager = new();

        // Start is called before the first frame update
        void Start()
        {
            _gridController = FindObjectOfType<GridController>();
        }

        public Creature GetMainCharacter()
        {
            if (_player == null)
                _player = GameRuntimeData.Instance.MainCharacterData.CreatureComponent;
            return _player;
        }

        public GridController GetGridController()
        {
            return _gridController;
        }

        public string GetLanguage()
        {
            return "PL";
        }

        public bool IsCurrentState(GameState state)
        {
            return stateManager.CurrentState.Equals(state);
        }

        public GameState GetCurrentState()
        {
            return stateManager.CurrentState;
        }

        public void OverrideState(GameState newState)
        {
            stateManager.CurrentState = newState;
        }
    }
}
