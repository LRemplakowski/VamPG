using System;
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

        private void OnEnable()
        {
            OnFadedIn += DisableOnFadedIn;
        }

        private void OnDisable()
        {
            OnFadedIn -= DisableOnFadedIn;
        }

        private void DisableOnFadedIn()
        {
            gameObject.SetActive(false);
        }

        internal void FadeOut()
        {
            gameObject.SetActive(true);
            animator.SetTrigger("Start");
        }

        internal void FadeIn()
        {
            animator.SetTrigger("Start");
        }

        public void NotifyFadedOut()
        {
            OnFadedOut?.Invoke();
        }

        public void NotifyFadedIn()
        {
            OnFadedIn?.Invoke();
        }
    }
}
