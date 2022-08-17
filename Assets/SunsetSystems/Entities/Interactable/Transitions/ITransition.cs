namespace SunsetSystems
{
    using SunsetSystems.Data;

    public interface ITransition
    {
        void MoveToScene(SceneLoadingData data);
    }
}