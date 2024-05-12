using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    public abstract class EquipableItem : BaseItem, IEquipableItem
    {
        [SerializeField]
        protected bool _overrideTooltipName = false;
        [SerializeField, ShowIf("_overrideTooltipName")]
        protected string _tooltipNameOverride = "Tooltip Override";
        public string TooltipName => _overrideTooltipName ? _tooltipNameOverride : Name;
        [field: SerializeField]
        public bool CanBeRemoved { get; private set; } = true;
        [field: SerializeField]
        public bool IsDefaultItem { get; private set; } = false;
    }
}
