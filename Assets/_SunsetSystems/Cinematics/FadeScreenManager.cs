using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Cinematics
{
    public class FadeScreenManager : MonoBehaviour
    {
        public static FadeScreenManager Instance { get; private set; }

        [Title("References")]
        [SerializeField, Required]
        private CanvasGroup _fadeScreenCanvasGroup;

        [Title("Config")]
        [SerializeField]
        private float _fadeTime = .5f;

        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void CycleFade(Action afterFadeOut = null)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
            _fadeScreenCanvasGroup.alpha = 0f;
            _fadeScreenCanvasGroup.blocksRaycasts = true;
            _fadeCoroutine = StartCoroutine(LerpAlphaOverTime(_fadeScreenCanvasGroup, _fadeTime, 1f, FadeInWhenCompleted));

            void FadeInWhenCompleted()
            {
                afterFadeOut?.Invoke();
                _fadeCoroutine = StartCoroutine(LerpAlphaOverTime(_fadeScreenCanvasGroup, _fadeTime, 0f, () => _fadeScreenCanvasGroup.blocksRaycasts = false));
            }
        }

        public void FadeOut(Action onAfterFadeOut = null)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
            _fadeScreenCanvasGroup.blocksRaycasts = true;
            _fadeCoroutine = StartCoroutine(LerpAlphaOverTime(_fadeScreenCanvasGroup, _fadeTime, 1f, onAfterFadeOut));
        }

        public void FadeIn(Action onAfterFadeIn = null)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(LerpAlphaOverTime(_fadeScreenCanvasGroup, _fadeTime, 0f, AfterFade));

            void AfterFade()
            {
                onAfterFadeIn?.Invoke();
                _fadeScreenCanvasGroup.blocksRaycasts = false;
            }
        }

        private static IEnumerator LerpAlphaOverTime(CanvasGroup canvasGroup, float fadeTime, float targetAlpha, Action onCoroutineCompleted = null)
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
