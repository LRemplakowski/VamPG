using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transitions.Manager
{
    [Serializable]
    public class TransitionAnimator : ExposableMonobehaviour
    {
        private Animator animator;

        public delegate void FadedOut();
        public static event FadedOut OnFadedOut;
        public delegate void FadedIn();
        public static event FadedIn OnFadedIn;

        private void Start()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
        }

        internal void FadeOut()
        {
            animator.SetTrigger("Start");
        }

        internal void FadeIn()
        {
            animator.SetTrigger("Start");
        }

        public void NotifyFadedOut()
        {
            if (OnFadedOut != null)
                OnFadedOut.Invoke();
        }

        public void NotifyFadedIn()
        {
            if (OnFadedIn != null)
                OnFadedIn.Invoke();
        }
    }
}
