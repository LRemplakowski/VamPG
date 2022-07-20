using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SunsetSystems.Loading;
using System.Threading.Tasks;

namespace SunsetSystems.Loading.UI
{
    internal class LoadingScreenController : MonoBehaviour
    {
        private const string LOADING_SCREEN_TAG = "LoadingScreen";

        [SerializeField]
        private Slider loadingBar;
        [SerializeField]
        private Image backgroundImage;
        [SerializeField]
        private TextMeshProUGUI loadingBarText;
        [SerializeField]
        private Button continueButton;
        [SerializeField]
        private FadeScreenAnimator transitionAnimator;

        public delegate void LoadingScreenHandler();
        public static event LoadingScreenHandler LoadingScreenEnabled;
        public static event LoadingScreenHandler LoadingScreenDisabled;

        private void OnEnable()
        {
            if (loadingBar == null)
                loadingBar = GetComponentInChildren<Slider>();
            if (backgroundImage == null)
                backgroundImage = GetComponent<Image>();
            if (loadingBarText == null)
                loadingBarText = GetComponentInChildren<TextMeshProUGUI>();
            if (continueButton == null)
                continueButton = GetComponentInChildren<Button>();
            loadingBar.value = 0f;
            continueButton.gameObject.SetActive(false);
            LoadingScreenEnabled?.Invoke();
        }

        private void Start()
        {
            transitionAnimator = FindObjectOfType<FadeScreenAnimator>(true);
        }

        public void SetUnloadingProgress(float value)
        {
            loadingBar.value = value / 2;
            loadingBarText.text = value * 50f + " %";
        }

        public void SetLoadingProgress(float value)
        {
            loadingBar.value = (value / 2) + .5f;
            loadingBarText.text = (value * 50f) + 50f + " %";
        }

        public void EnableContinue()
        {
            Debug.Log("enabling continue button");
            continueButton.gameObject.SetActive(true);
        }

        public async void OnContinue()
        {
            await transitionAnimator.FadeOut(.5f);
            DisableLoadingScreen();
            await transitionAnimator.FadeIn(.5f);
        }

        private void DisableLoadingScreen()
        {
            this.gameObject.SetActive(false);
            LoadingScreenDisabled?.Invoke();
        }
    }
}
