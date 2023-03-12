using System;
using UnityEngine;

namespace SunsetSystems.Entities.Interactable
{
    public class Doors : InteractableEntity
    {
        [SerializeField]
        private Animator _animator;
        [field: SerializeField]
        public bool ReverseAnimation { get; private set; }

        protected override void Start()
        {
            base.Start();
            if (!_animator)
                _animator = GetComponentInChildren<Animator>();
        }

        protected override void HandleInteraction()
        {
            Debug.Log("Switching door animation state!");
            if (ReverseAnimation)
                _animator?.SetTrigger("DoorSwitchReverse");
            else
                _animator?.SetTrigger("DoorSwitch");
        }

        public override object GetPersistenceData()
        {
            DoorsPersistenceData persistenceData = new(base.GetPersistenceData() as InteractableEntityPersistenceData);
            DoorsPersistenceData.AnimationStateData animationData = new();
            animationData.AnimatorState = _animator.GetCurrentAnimatorStateInfo(0);
            animationData.AnimatorStateNext = _animator.GetNextAnimatorStateInfo(0);
            animationData.AnimatorTransition = _animator.GetAnimatorTransitionInfo(0);
            persistenceData.AnimationData = animationData;
            return persistenceData;
        }

        public override void InjectPersistenceData(object data)
        {
            base.InjectPersistenceData(data);
            DoorsPersistenceData.AnimationStateData animationData = (data as DoorsPersistenceData).AnimationData;
            _animator.Play(animationData.AnimatorState.shortNameHash, 0, animationData.AnimatorState.normalizedTime);
            _animator.Update(0f);
            _animator.CrossFadeInFixedTime(animationData.AnimatorStateNext.shortNameHash, animationData.AnimatorTransition.duration, 0, 0f, animationData.AnimatorTransition.normalizedTime);
        }

        [Serializable]
        protected class DoorsPersistenceData : InteractableEntityPersistenceData
        {
            public AnimationStateData AnimationData;

            public DoorsPersistenceData(InteractableEntityPersistenceData persistenceData)
            {
                GameObjectActive = persistenceData.GameObjectActive;
                Interactable = persistenceData.Interactable;
            }

            public DoorsPersistenceData()
            {

            }

            [Serializable]
            public struct AnimationStateData
            {
                public AnimatorStateInfo AnimatorState, AnimatorStateNext;
                public AnimatorTransitionInfo AnimatorTransition;
            }
        }
    }
}
