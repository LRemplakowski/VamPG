using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Utils
{
    public static class CoroutineUtility
    {
        public static IEnumerator LerpAlphaOverTime(CanvasGroup canvasGroup, float fadeTime, float targetAlpha, Action onCoroutineCompleted = null)
        {
            float startAlpha = Mathf.Abs(targetAlpha - 1);
            float timeElapsed = Mathf.InverseLerp(startAlpha, targetAlpha, canvasGroup.alpha) / fadeTime;
            while (timeElapsed < fadeTime)
            {
                timeElapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / fadeTime);
                yield return null;
            }
            canvasGroup.alpha = targetAlpha;
            onCoroutineCompleted?.Invoke();
        }
    }
}
