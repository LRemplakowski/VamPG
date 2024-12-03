using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Abilities;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    public abstract class EquipableItem : BaseItem, IEquipableItem, IAbilitySource
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
        [SerializeField]
        protected List<IAbility> _grantedAbilities = new();

        public IEnumerable<IAbility> GetAbilities()
        {
            return _grantedAbilities;
        }
    }
}
