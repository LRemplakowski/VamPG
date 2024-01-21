using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SunsetSystems.Persistence;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using SunsetSystems.Core.AddressableManagement;

namespace SunsetSystems.Core.SceneLoading.UI
{
    public class LoadingScreenController : SerializedMonoBehaviour
    {
        [Title("Runtime References")]
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
        [Title("Asset References")]
        [SerializeField]
        private LoadingScreenProviderConfig loadingScreenProviderConfig;

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
        }

        private void OnDisable()
        {
            loadingScreenProviderConfig.ReleaseLoadingScreens();           
        }

        private void Start()
        {
            transitionAnimator = FindObjectOfType<FadeScreenAnimator>(true);
        }

        public void SetUnloadingProgress(float value)
        {
            loadingBar.value = value / 2;
            //loadingBarText.text = value * 50f + " %";
        }

        public void SetLoadingProgress(float value)
        {
            loadingBar.value = (value / 2) + .5f;
            //loadingBarText.text = (value * 50f) + 50f + " %";
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
        }

        public async Task LoadRandomLoadingScreen()
        {
            backgroundImage.sprite = await loadingScreenProviderConfig.GetRandomLoadingScreenAsync();
        }
    }
}
