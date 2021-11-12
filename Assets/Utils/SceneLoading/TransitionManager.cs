using System;
using System.Collections;
using System.Collections.Generic;
using Transitions.Data;
using UnityEngine;
using Utils.Scenes;
using Utils.Singleton;

namespace Transitions.Manager
{
    public class TransitionManager : Singleton<TransitionManager>
    {
        private TransitionAnimator _animator;
        private TransitionAnimator Animator
        {
            get
            {
                if (_animator == null)
                    _animator = FindObjectOfType<TransitionAnimator>(true);
                return _animator;
            }
        }


        private TransitionData currentTransition;

        private LoadingScreenController loadingScreenController;

        #region Enable & Disable
        private void OnEnable()
        {
            TransitionAnimator.OnFadedIn += OnFadedIn;
            TransitionAnimator.OnFadedOut += OnFadedOut;
        }

        private void OnDisable()
        {
            TransitionAnimator.OnFadedIn -= OnFadedIn;
            TransitionAnimator.OnFadedOut -= OnFadedOut;
        }
        #endregion

        public void PerformTransition(TransitionData data)
        {
            if (loadingScreenController == null)
                loadingScreenController = FindObjectOfType<LoadingScreenController>(true);
            currentTransition = data;
            SetTransitionPanelActive(true);
            Animator.FadeOut();
        }
        private void OnFadedIn()
        {
            SetTransitionPanelActive(false);
        }

        private void OnFadedOut()
        {
            loadingScreenController.gameObject.SetActive(!loadingScreenController.gameObject.activeSelf);
            if (currentTransition != null)
                SceneLoader.Instance.LoadScene(currentTransition);
            Animator.FadeIn();
            currentTransition = null;
        }

        public void SetTransitionPanelActive(bool active)
        {
            Animator.gameObject.SetActive(active);
        }
    }
}
