namespace Transitions
{
    using Transitions.Data;

    public interface ITransition
    {
        void MoveToScene(SceneLoadingData data);
    }
}