namespace Transitions
{
    using Transitions.Data;

    public interface ITransition
    {
        void MoveToScene(NameTransition data);

        void MoveToScene(IndexTransition data);
    }
}