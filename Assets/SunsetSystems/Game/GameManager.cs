using Entities.Characters;
using SunsetSystems.Data;
using SunsetSystems.Utils;

namespace SunsetSystems.Game
{
    public class GameManager : Singleton<GameManager>
    {
        private static Creature _player;
        private static GridController _gridController;
        public static GameState CurrentState { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            _gridController = FindObjectOfType<GridController>();
        }

        public static Creature GetMainCharacter()
        {
            if (_player == null)
                _player = GameRuntimeData.Instance.MainCharacterData.CreatureComponent;
            return _player;
        }

        public static GridController GetGridController()
        {
            return _gridController;
        }

        public static string GetLanguage()
        {
            return "PL";
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
