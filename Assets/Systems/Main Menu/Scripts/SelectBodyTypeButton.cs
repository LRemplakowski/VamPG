using Transitions.Manager;
using UnityEngine;

namespace Systems.MainMenu.UI
{
    public class SelectBodyTypeButton : MonoBehaviour
    {
        private TransitionAnimator transitionAnimator;
        private GameInitializer gameInitializer;
        [SerializeField]
        private GameObject statsAssignmentObject;
        [SerializeField]
        private GameObject bodyTypeSelectionParent;
        [SerializeField]
        private BodyType associatedBodyType;

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
            DoLoadStatsAssignment();
        }

        public void SelectBodyType()
        {
            DoSelectBodyType();
            transitionAnimator.FadeOut();
        }

        private void DoSelectBodyType()
        {
            gameInitializer.SelectBodyType(associatedBodyType);
        }

        private void DoLoadStatsAssignment()
        {
            if (statsAssignmentObject)
                statsAssignmentObject.SetActive(true);
            bodyTypeSelectionParent.SetActive(false);
            transitionAnimator.FadeIn();
        }
    }
}
