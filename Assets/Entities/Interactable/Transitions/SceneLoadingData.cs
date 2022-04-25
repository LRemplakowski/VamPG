using SunsetSystems.Loading;
using System;

namespace SunsetSystems.Data
{ 
    public abstract class SceneLoadingData
    {
        public readonly TransitionType transitionType;
        public readonly string targetEntryPointTag;
        public readonly Action[] preLoadingActions;

        public SceneLoadingData(TransitionType transitionType, string targetEntryPointTag, params Action[] preLoadingActions)
        {
            this.transitionType = transitionType;
            this.targetEntryPointTag = targetEntryPointTag;
            this.preLoadingActions = preLoadingActions;
        }

        public abstract object Get();
    }

    public class IndexLoadingData : SceneLoadingData
    {
        private readonly int sceneIndex;

        public IndexLoadingData(int sceneIndex, string targetEntryPointTag, params Action[] preLoadingActions)
            : base(TransitionType.index, targetEntryPointTag, preLoadingActions)
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

        public NameLoadingData(string sceneName, string targetEntryPointTag, params Action[] preLoadingActions)
            : base(TransitionType.name, targetEntryPointTag, preLoadingActions)
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