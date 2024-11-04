using Sirenix.OdinInspector;
using SunsetSystems.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SunsetSystems.WorldMap
{
    public abstract class AbstractWorldMapConfirmationWindow : SerializedMonoBehaviour, IWorldMapConfirmationWindow, ICancelHandler, ISubmitHandler, IPointerClickHandler
    {
        [Title("References")]
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Button _confirmationButton;
        [SerializeField]
        private Button _cancelButton;
        [SerializeField]
        protected WorldMapUI _uiManager;

        private Coroutine _uiLerpCoroutine;

        public void HideConfirmationWindow()
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            if (_uiLerpCoroutine != null)
                StopCoroutine(_uiLerpCoroutine);
            _uiLerpCoroutine = StartCoroutine(CoroutineUtility.LerpAlphaOverTime(_canvasGroup, .25f, 0f, () => gameObject.SetActive(false)));
        }

        public void ShowConfirmationWindow(IWorldMapData worldMapData)
        {
            gameObject.SetActive(true);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            if (_uiLerpCoroutine != null)
                StopCoroutine(_uiLerpCoroutine);
            _uiLerpCoroutine = StartCoroutine(CoroutineUtility.LerpAlphaOverTime(_canvasGroup, .25f, 1f));
            HandleWorldMapData(worldMapData);
        }

        public void OnCancel(BaseEventData eventData)
        {
            OnReject();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                OnReject();
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            OnConfirm();
        }

        protected abstract void HandleWorldMapData(IWorldMapData worldMapData);
        public abstract void OnConfirm();
        public abstract void OnReject();
    }
}
