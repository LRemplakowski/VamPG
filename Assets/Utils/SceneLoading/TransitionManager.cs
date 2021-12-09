using Transitions.Data;
using UnityEngine;
using Utils.Scenes;
using Utils.Singleton;

namespace Transitions.Manager
{
    public class TransitionManager : Singleton<TransitionManager>
    {
        [SerializeField]
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

        private void Start()
        {
            if (loadingScreenController == null)
                loadingScreenController = FindObjectOfType<LoadingScreenController>(true);
        }

        public void PerformTransition(TransitionData data)
        {
            if (loadingScreenController == null)
                loadingScreenController = FindObjectOfType<LoadingScreenController>(true);
            currentTransition = data;
            TransitionAnimator.OnFadedIn += OnFadedIn;
            TransitionAnimator.OnFadedOut += OnFadedOut;
            Animator.FadeOut();
        }

        private void OnFadedIn()
        {
            TransitionAnimator.OnFadedIn -= OnFadedIn;
            TransitionAnimator.OnFadedOut -= OnFadedOut;
        }

        private void OnFadedOut()
        {
            if (loadingScreenController)
                loadingScreenController.gameObject.SetActive(!loadingScreenController.gameObject.activeSelf);
            if (currentTransition != null)
                SceneLoader.Instance.LoadScene(currentTransition);
            Animator.FadeIn();
            currentTransition = null;
        }
    }
}
