using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Animation
{
    [RequireComponent(typeof(Animator))]
    public class AnimationSelector : StateMachineBehaviour
    {
        [SerializeField]
        private int _stateCount;
        [SerializeField]
        private string animatorParameter = "selected_animation";

        private static readonly System.Random _random = new();
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            animator.SetInteger("animatorParametern", _random.Next(0, _stateCount));
        }
    }
}
