using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SunsetSystems.WorldMap
{
    public class WorldMapTravelConfirmation : SerializedMonoBehaviour, ICancelHandler, ISubmitHandler, IPointerClickHandler
    {
        [Title("References")]
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Button _confirmationButton;
        [SerializeField]
        private Button _cancelButton;
        [SerializeField]
        private WorldMapUI _uiManager;

        private Coroutine _uiLerpCoroutine;

        public void HideConfirmationWindow()
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            if (_uiLerpCoroutine != null)
                StopCoroutine(_uiLerpCoroutine);
            _uiLerpCoroutine = StartCoroutine(CoroutineUtility.LerpAlphaOverTime(_canvasGroup, .25f, 0f, () => gameObject.SetActive(false)));
        }

        public void ShowConfirmationWindow()
        {
            gameObject.SetActive(true);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            if (_uiLerpCoroutine != null)
                StopCoroutine(_uiLerpCoroutine);
            _uiLerpCoroutine = StartCoroutine(CoroutineUtility.LerpAlphaOverTime(_canvasGroup, .25f, 1f));
        }

        public void ConfirmTravel()
        {
            _uiManager.ConfirmTravelToSelectedArea();
        }

        public void OnCancel(BaseEventData eventData)
        {
            HideConfirmationWindow();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                HideConfirmationWindow();
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            ConfirmTravel();
        }
    }
}
