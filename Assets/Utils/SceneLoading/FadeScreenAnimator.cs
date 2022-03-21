using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Transitions.Manager
{
    [Serializable]
    public class FadeScreenAnimator : ExposableMonobehaviour
    {
        private Image fadePanel;

        private void Start()
        {
            if (fadePanel == null)
                fadePanel = GetComponent<Image>();
            _ = FadeIn(.5f);
        }

        internal async Task FadeOut(float fadeTime)
        {
            gameObject.SetActive(true);
            Color color = fadePanel.color;
            color.a = 0;
            fadePanel.color = color;
            float lerpTime = 0;
            while (fadePanel.color.a <= 0.99f)
            {
                color = fadePanel.color;
                color = Color.Lerp(color, Color.black, lerpTime / fadeTime);
                lerpTime += Time.deltaTime;
                fadePanel.color = color;
                await Task.Yield();
            }
            fadePanel.color = Color.black;
        }

        internal async Task FadeIn(float fadeTime)
        {
            Color color = fadePanel.color;
            color.a = 1f;
            fadePanel.color = color;
            float lerpTime = 0;
            while (fadePanel.color.a >= 0.01f)
            {
                color = fadePanel.color;
                color = Color.Lerp(color, Color.clear, lerpTime / fadeTime);
                lerpTime += Time.deltaTime;
                fadePanel.color = color;
                await Task.Yield();
            }
            fadePanel.color = Color.clear;
            gameObject.SetActive(false);
        }
    }
}
