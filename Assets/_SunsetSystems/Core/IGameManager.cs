namespace SunsetSystems.Game
{
    public interface IGameManager
    {
        GameState CurrentState { get; set; }

        string GetLanguage();

        bool IsCurrentState(GameState state);
    }
}
