using Entities.Characters;
using UnityEngine.SceneManagement;

namespace Transitions.Data
{ 
    public abstract class SceneLoadingData
    {
        public readonly TransitionType transitionType;
        public readonly string targetEntryPointTag;

        public SceneLoadingData(TransitionType transitionType, string targetEntryPointTag)
        {
            this.transitionType = transitionType;
            this.targetEntryPointTag = targetEntryPointTag;
        }

        public abstract object Get();
    }

    public class IndexLoadingData : SceneLoadingData
    {
        private readonly int sceneIndex;
        
        public IndexLoadingData(int sceneIndex, string targetEntryPointTag) 
            : base(TransitionType.index, targetEntryPointTag)
        {
            this.sceneIndex = sceneIndex;
        }

        public override object Get()
        {
            return sceneIndex;
        }
    }

    public class NameLoadingData : SceneLoadingData
    {
        private readonly string sceneName;

        public NameLoadingData(string sceneName, string targetEntryPointTag) 
            : base(TransitionType.name, targetEntryPointTag)
        {
            this.sceneName = sceneName;
        }

        public override object Get()
        {
            return sceneName;
        }
    }
}