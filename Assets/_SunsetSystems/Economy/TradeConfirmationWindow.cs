using TMPro;
using UnityEngine;

namespace SunsetSystems.Economy.UI
{
    public class TradeConfirmationWindow : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _confirmationWindowCanvasGroup;
        [SerializeField]
        private TextMeshProUGUI _confirmationText;

        public void SetConfirmationWindowVisible(bool visible)
        {
            _confirmationWindowCanvasGroup.alpha = visible ? 1 : 0;
            _confirmationWindowCanvasGroup.interactable = visible;
            _confirmationWindowCanvasGroup.blocksRaycasts = visible;
        }

        public void UpdateTradeConfirmationText(ITradeOfferViewData viewData)
        {
            var localizedText = GetLocalizedText(viewData.Title, viewData.OfferPrice);
            _confirmationText.text = localizedText;
        }

        private string GetLocalizedText(string itemName, float value)
        {
            return $"Are you sure you want to buy {itemName} for {value:0.00}$?";
        }
    }
}