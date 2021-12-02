using Systems.Journal;
using Transitions.Manager;
using UnityEngine;

namespace Systems.MainMenu.UI
{
    public class SelectBackgroundButton : MonoBehaviour
    {
        private TransitionAnimator transitionAnimator;
        private GameInitializer gameInitializer;
        [SerializeField]
        private GameObject bodyTypeSelectionObject;
        [SerializeField]
        private GameObject backgroundSelectionParent;
        [SerializeField]
        private PlayerCharacterBackground associatedBackground;

        private void Start()
        {
            if (transitionAnimator == null)
                transitionAnimator = FindObjectOfType<TransitionAnimator>(true);
            if (gameInitializer == null)
                gameInitializer = FindObjectOfType<GameInitializer>();
        }

        #region Enable&Disable
        private void OnEnable()
        {
            TransitionAnimator.OnFadedOut += OnFadedOut;
        }

        private void OnDisable()
        {
            TransitionAnimator.OnFadedOut -= OnFadedOut;
        }
        #endregion

        private void OnFadedOut()
        {
            DoLoadBodyTypeSelection();
        }

        public void SelectBackground()
        {
            DoSelectBackground();
            transitionAnimator.FadeOut();
        }

        private void DoSelectBackground()
        {
            gameInitializer.SelectBackground(associatedBackground);
        }

        private void DoLoadBodyTypeSelection()
        {
            if (bodyTypeSelectionObject)
                bodyTypeSelectionObject.SetActive(true);
            backgroundSelectionParent.SetActive(false);
            transitionAnimator.FadeIn();
        }
    }
}
