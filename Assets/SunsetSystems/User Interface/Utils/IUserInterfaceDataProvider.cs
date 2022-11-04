namespace SunsetSystems.UI.Utils
{
    public interface IGameDataProvider<T> where T : IGameDataProvider<T>
    {
        T Data { get; }
    }
}
