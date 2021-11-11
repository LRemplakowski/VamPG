namespace Transitions.Data
{ 
    public abstract class TransitionData
    {
        public readonly TransitionType transitionType;

        public TransitionData(TransitionType transitionType)
        {
            this.transitionType = transitionType;
        }

        public abstract object get();
    }

    public class IndexTransition : TransitionData
    {
        private readonly int sceneIndex;
        
        public IndexTransition(int sceneIndex) : base(TransitionType.index)
        {
            this.sceneIndex = sceneIndex;
        }

        public override object get()
        {
            return sceneIndex;
        }
    }

    public class NameTransition : TransitionData
    {
        private readonly string sceneName;

        public NameTransition(string sceneName) : base(TransitionType.name)
        {
            this.sceneName = sceneName;
        }

        public override object get()
        {
            return sceneName;
        }
    }
}