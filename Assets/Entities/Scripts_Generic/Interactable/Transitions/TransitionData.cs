namespace Transitions.Data
{ 
    public abstract class TransitionData<T>
    {
        public abstract T get();
    }

    public class IndexTransition : TransitionData<int>
    {
        private readonly int sceneIndex;
        
        public IndexTransition(int sceneIndex)
        {
            this.sceneIndex = sceneIndex;
        }

        public override int get()
        {
            return sceneIndex;
        }
    }

    public class NameTransition : TransitionData<string>
    {
        private readonly string sceneName;

        public NameTransition(string sceneName)
        {
            this.sceneName = sceneName;
        }

        public override string get()
        {
            return sceneName;
        }
    }
}