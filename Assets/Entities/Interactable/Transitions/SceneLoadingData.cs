using SunsetSystems.Loading;
using System;

namespace SunsetSystems.Data
{
    public abstract class SceneLoadingData
    {
        public readonly TransitionType transitionType;
        public readonly string targetEntryPointTag;
        public readonly string cameraBoundingBoxTag;
        public readonly Action[] preLoadingActions;

        public SceneLoadingData(TransitionType transitionType, string targetEntryPointTag, string cameraBoundingBoxTag, params Action[] preLoadingActions)
        {
            this.transitionType = transitionType;
            this.targetEntryPointTag = targetEntryPointTag;
            this.cameraBoundingBoxTag = cameraBoundingBoxTag;
            this.preLoadingActions = preLoadingActions;
        }

        public abstract object Get();
    }

    public class IndexLoadingData : SceneLoadingData
    {
        private readonly int sceneIndex;

        public IndexLoadingData(int sceneIndex, string targetEntryPointTag, string cameraBoundingBoxTag, params Action[] preLoadingActions)
            : base(TransitionType.indexTransition, targetEntryPointTag, cameraBoundingBoxTag, preLoadingActions)
        {
            this.sceneIndex = sceneIndex;
        }

        public IndexLoadingData(int sceneIndex, string targetEntryPointTag, string cameraBoundingBoxTag)
            : this(sceneIndex, targetEntryPointTag, cameraBoundingBoxTag, null)
        { }

        public override object Get()
        {
            return sceneIndex;
        }
    }

    public class NameLoadingData : SceneLoadingData
    {
        private readonly string sceneName;

        public NameLoadingData(string sceneName, string targetEntryPointTag, string cameraBoundingBoxTag, params Action[] preLoadingActions)
            : base(TransitionType.nameTransition, targetEntryPointTag, cameraBoundingBoxTag, preLoadingActions)
        {
            this.sceneName = sceneName;
        }

        public NameLoadingData(string sceneName, string targetEntryPointTag, string cameraBoundingBoxTag)
            : this(sceneName, targetEntryPointTag, cameraBoundingBoxTag, null)
        { }

        public override object Get()
        {
            return sceneName;
        }
    }
}