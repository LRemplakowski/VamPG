using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Abilities;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    public abstract class EquipableItem : BaseItem, IEquipableItem
    {
        [SerializeField, BoxGroup("Equipable Item")]
        protected bool _overrideTooltipName = false;
        [SerializeField, ShowIf("_overrideTooltipName"), BoxGroup("Equipable Item")]
        protected string _tooltipNameOverride = "Tooltip Override";
        public string TooltipName => _overrideTooltipName ? _tooltipNameOverride : Name;
        [field: SerializeField, BoxGroup("Equipable Item")]
        public bool CanBeRemoved { get; private set; } = true;
        [field: SerializeField, BoxGroup("Equipable Item")]
        public bool IsDefaultItem { get; private set; } = false;
    }
}
