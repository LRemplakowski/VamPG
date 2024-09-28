using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using static SunsetSystems.UI.InventoryItemDisplay;

namespace SunsetSystems.Tooltips
{
    public class InventoryNameplate : AbstractTooltip<InventoryNameplateData>
    {
        [Title("Config")]
        [SerializeField, Required]
        private TextMeshProUGUI _nameplateText;

        protected override void UpdateTooltipFromContext(InventoryNameplateData context)
        {
            _nameplateText.text = context.ItemNameText;
        }

        protected override void DoCleanUp()
        {
            _nameplateText.text = "";
        }
    }
}
