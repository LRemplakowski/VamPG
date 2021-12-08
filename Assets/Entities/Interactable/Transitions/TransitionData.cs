using Entities.Characters;
using UnityEngine.SceneManagement;

namespace Transitions.Data
{ 
    public abstract class TransitionData
    {
        public readonly TransitionType transitionType;
        public readonly string targetEntryPointTag;
        public readonly LoadSceneMode loadSceneMode;

        public TransitionData(TransitionType transitionType, string targetEntryPointTag)
        {
            this.transitionType = transitionType;
            this.targetEntryPointTag = targetEntryPointTag;
            loadSceneMode = LoadSceneMode.Additive;
        }

        public TransitionData(TransitionType transitionType, string targetEntryPointTag, LoadSceneMode loadSceneMode) 
            : this(transitionType, targetEntryPointTag)
        {
            this.loadSceneMode = loadSceneMode;
        }

        public abstract object get();
    }

    public class IndexTransition : TransitionData
    {
        private readonly int sceneIndex;
        
        public IndexTransition(int sceneIndex, string targetEntryPointTag) 
            : base(TransitionType.index, targetEntryPointTag)
        {
            this.sceneIndex = sceneIndex;
        }

        public IndexTransition(int sceneIndex, string targetEntryPointTag, LoadSceneMode loadSceneMode) 
            : base(TransitionType.index, targetEntryPointTag, loadSceneMode)
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

        public NameTransition(string sceneName, string targetEntryPointTag) 
            : base(TransitionType.name, targetEntryPointTag)
        {
            this.sceneName = sceneName;
        }

        public NameTransition(string sceneName, string targetEntryPointTag, LoadSceneMode loadSceneMode)
    : base(TransitionType.name, targetEntryPointTag, loadSceneMode)
        {
            this.sceneName = sceneName;
        }

        public override object get()
        {
            return sceneName;
        }
    }
}