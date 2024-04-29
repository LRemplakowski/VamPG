using SunsetSystems.Core.SceneLoading;

namespace SunsetSystems
{   

    public interface ITransition
    {
        void MoveToScene(SceneLoadingDataAsset data);
    }
}