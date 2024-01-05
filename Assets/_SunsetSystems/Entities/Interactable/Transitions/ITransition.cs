using SunsetSystems.Core.SceneLoading;

namespace SunsetSystems
{   

    public interface ITransition
    {
        void MoveToScene(SceneLoadingData data);
    }
}