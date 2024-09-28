using SunsetSystems.UI;

namespace SunsetSystems.Tooltips
{
    public class InventoryNameplateManager : AbstractTooltipManager<InventoryItemDisplay.InventoryNameplateData, InventoryNameplate>
    {
        private void OnEnable()
        {
            InventoryItemDisplay.OnPointerEnterItem += OnPointerEnterItem;
            InventoryItemDisplay.OnPointerExitItem += OnPointerExitItem;
        }

        private void OnDisable()
        {
            InventoryItemDisplay.OnPointerEnterItem -= OnPointerEnterItem;
            InventoryItemDisplay.OnPointerExitItem -= OnPointerExitItem;
        }

        private void OnPointerEnterItem(InventoryItemDisplay display)
        {
            ShowTooltip(display.TooltipData);
        }

        private void OnPointerExitItem(InventoryItemDisplay display)
        {
            HideTooltip(display.TooltipData);
        }
    }
}
