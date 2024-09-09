using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Utils;
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
            _fadeCoroutine = StartCoroutine(CoroutineUtility.LerpAlphaOverTime(_fadeScreenCanvasGroup, _fadeTime, 1f, FadeInWhenCompleted));

            void FadeInWhenCompleted()
            {
                afterFadeOut?.Invoke();
                _fadeCoroutine = StartCoroutine(CoroutineUtility.LerpAlphaOverTime(_fadeScreenCanvasGroup, _fadeTime, 0f, () => _fadeScreenCanvasGroup.blocksRaycasts = false));
            }
        }

        public void FadeOut(Action onAfterFadeOut = null)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
            _fadeScreenCanvasGroup.blocksRaycasts = true;
            _fadeCoroutine = StartCoroutine(CoroutineUtility.LerpAlphaOverTime(_fadeScreenCanvasGroup, _fadeTime, 1f, onAfterFadeOut));
        }

        public void FadeIn(Action onAfterFadeIn = null)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(CoroutineUtility.LerpAlphaOverTime(_fadeScreenCanvasGroup, _fadeTime, 0f, AfterFade));

            void AfterFade()
            {
                onAfterFadeIn?.Invoke();
                _fadeScreenCanvasGroup.blocksRaycasts = false;
            }
        }
    }
}
