using SunsetSystems.LevelManagement;
using System;

namespace SunsetSystems.Data
{
    public abstract class LevelLoadingData
    {
        public readonly TransitionType transitionType;
        public readonly string targetEntryPointTag;
        public readonly string cameraBoundingBoxTag;

        public LevelLoadingData(TransitionType transitionType, string targetEntryPointTag, string cameraBoundingBoxTag)
        {
            this.transitionType = transitionType;
            this.targetEntryPointTag = targetEntryPointTag;
            this.cameraBoundingBoxTag = cameraBoundingBoxTag;
        }

        public abstract object Get();
    }

    public class IndexLoadingData : LevelLoadingData
    {
        private readonly int sceneIndex;

        public IndexLoadingData(int sceneIndex, string targetEntryPointTag, string cameraBoundingBoxTag)
            : base(TransitionType.indexTransition, targetEntryPointTag, cameraBoundingBoxTag)
        {
            this.sceneIndex = sceneIndex;
        }

        public override object Get()
        {
            return sceneIndex;
        }
    }

    public class NameLoadingData : LevelLoadingData
    {
        private readonly string sceneName;

        public NameLoadingData(string sceneName, string targetEntryPointTag, string cameraBoundingBoxTag)
            : base(TransitionType.nameTransition, targetEntryPointTag, cameraBoundingBoxTag)
        {
            this.sceneName = sceneName;
        }

        public override object Get()
        {
            return sceneName;
        }
    }
}