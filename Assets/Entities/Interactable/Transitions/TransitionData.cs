﻿using Entities.Characters;

namespace Transitions.Data
{ 
    public abstract class TransitionData
    {
        public readonly TransitionType transitionType;
        public readonly string targetEntryPointTag;

        public TransitionData(TransitionType transitionType, string targetEntryPointTag, CreatureAsset[] currentParty)
        {
            this.transitionType = transitionType;
            this.targetEntryPointTag = targetEntryPointTag;
        }

        public abstract object get();
    }

    public class IndexTransition : TransitionData
    {
        private readonly int sceneIndex;
        
        public IndexTransition(int sceneIndex, string targetEntryPointTag, CreatureAsset[] currentParty) 
            : base(TransitionType.index, targetEntryPointTag, currentParty)
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

        public NameTransition(string sceneName, string targetEntryPointTag, CreatureAsset[] currentParty) 
            : base(TransitionType.name, targetEntryPointTag, currentParty)
        {
            this.sceneName = sceneName;
        }

        public override object get()
        {
            return sceneName;
        }
    }
}