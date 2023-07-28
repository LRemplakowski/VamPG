using Redcode.Awaiting;
using SunsetSystems.Utils;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Core.SceneLoading.UI
{
    public class FadeScreenAnimator : MonoBehaviour
    {
        [SerializeField]
        private Image fadePanel;

        private void Awake()
        {
            if (fadePanel == null)
                fadePanel = GetComponent<Image>();
            _ = FadeIn(.5f);
        }

        public async Task FadeOut(float fadeTime, Action action)
        {
            action.Invoke();
            await FadeOut(fadeTime);
        }

        public async Task FadeOut(float fadeTime)
        {
            gameObject.SetActive(true);
            fadePanel.color = Color.clear;
            float lerpTime = 0;
            while (lerpTime / fadeTime < 1f)
            {
                lerpTime += Time.deltaTime;
                fadePanel.color = Color.Lerp(Color.clear, Color.black, lerpTime / fadeTime);
                await new WaitForUpdate();
            }
            fadePanel.color = Color.black;
        }

        public async Task FadeIn(float fadeTime)
        {
            float lerpTime = 0;
            while (lerpTime / fadeTime < 1f)
            {
                lerpTime += Time.deltaTime;
                fadePanel.color = Color.Lerp(Color.black, Color.clear, lerpTime / fadeTime);
                await new WaitForUpdate();
            }
            fadePanel.color = Color.clear;
            gameObject.SetActive(false);
        }
    }
}
