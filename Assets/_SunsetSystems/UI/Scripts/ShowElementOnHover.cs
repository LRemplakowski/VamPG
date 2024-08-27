using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SunsetSystems.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ShowElementOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Title("Config")]
        [SerializeField, Min(0.01f)]
        private float _showHideTime = .5f;
        [SerializeField]
        private float _holdBeforeFadeOut = 3f;
        [Title("References")]
        [SerializeField]
        private CanvasGroup _canvasGroup;

        private bool _showElement = false;
        private float _timeBeforeFade = 0f;

        private Coroutine _flipTriggerCoroutine;

        private void Awake()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
        }

        private void Update()
        {
            if (_showElement)
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, 1f, Time.deltaTime / _showHideTime);
            }
            else
            {
                if (_timeBeforeFade > 0f)
                    _timeBeforeFade -= Time.deltaTime;
                if (_timeBeforeFade <= 0f)
                    _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, 0f, Time.deltaTime / _showHideTime);
            }
        }

        public void TriggerShowElement()
        {
            _showElement = true;
            _timeBeforeFade = _holdBeforeFadeOut;
            if (_flipTriggerCoroutine != null)
                StopCoroutine(_flipTriggerCoroutine);
            StartCoroutine(FlipTriggerAfterTime(_showHideTime));

            IEnumerator FlipTriggerAfterTime(float time)
            {
                yield return new WaitForSeconds(time);
                _showElement = false;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _showElement = true;
            _timeBeforeFade = _holdBeforeFadeOut;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _showElement = false;
        }
    }
}
