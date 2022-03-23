using System;

namespace Transitions.Data
{ 
    public abstract class SceneLoadingData
    {
        public readonly TransitionType transitionType;
        public readonly string targetEntryPointTag;
        public readonly Action preLoadingAction;

        public SceneLoadingData(TransitionType transitionType, string targetEntryPointTag, Action preLoadingAction)
        {
            this.transitionType = transitionType;
            this.targetEntryPointTag = targetEntryPointTag;
            this.preLoadingAction = preLoadingAction;
        }

        public abstract object Get();
    }

    public class IndexLoadingData : SceneLoadingData
    {
        private readonly int sceneIndex;

        public IndexLoadingData(int sceneIndex, string targetEntryPointTag, Action preLoadingAction)
            : base(TransitionType.index, targetEntryPointTag, preLoadingAction)
        {
            this.sceneIndex = sceneIndex;
        }

        public IndexLoadingData(int sceneIndex, string targetEntryPointTag)
            : this(sceneIndex, targetEntryPointTag, null)
        { }

        public override object Get()
        {
            return sceneIndex;
        }
    }

    public class NameLoadingData : SceneLoadingData
    {
        private readonly string sceneName;

        public NameLoadingData(string sceneName, string targetEntryPointTag, Action preLoadingAction)
            : base(TransitionType.name, targetEntryPointTag, preLoadingAction)
        {
            this.sceneName = sceneName;
        }

        public NameLoadingData(string sceneName, string targetEntryPointTag) 
            : this(sceneName, targetEntryPointTag, null)
        { }

        public override object Get()
        {
            return sceneName;
        }
    }
}