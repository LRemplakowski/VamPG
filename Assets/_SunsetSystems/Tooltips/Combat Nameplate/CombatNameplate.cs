using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Tooltips
{
    public class CombatNameplate : AbstractTooltip<CombatNameplateData>
    {
        [Title("References")]
        [SerializeField, Required]
        private TextMeshProUGUI _name;
        [SerializeField, Required]
        private TextMeshProUGUI _healthValue;
        [SerializeField, Required]
        private Slider _healthSlider;
        [SerializeField, Required]
        private Image _enemyTypeImage;
        [Title("Config")]
        [SerializeField, DictionaryDrawerSettings(IsReadOnly = true)]
        private Dictionary<Inventory.AbilityRange, Sprite> _weaponTypeIcons = new();

        private void OnValidate()
        {
            foreach (Inventory.AbilityRange weaponType in Enum.GetValues(typeof(Inventory.AbilityRange)))
            {
                _weaponTypeIcons.TryAdd(weaponType, null);
            }
        }

        protected override void UpdateTooltipFromContext(CombatNameplateData context)
        {
            if (context.CurrentHP <= 0)
            {
                gameObject.SetActive(false);
            }
            _name.text = context.Name;
            _healthValue.text = $"{context.CurrentHP}/{context.MaxHP}";
            _healthSlider.value = context.HealthPercentage;
            if (_weaponTypeIcons.TryGetValue(context.CurrentWeapon, out var icon))
                _enemyTypeImage.sprite = icon;
        }

        protected override void DoCleanUp()
        {
            _name.text = "";
            _healthValue.text = "";
            _healthSlider.value = 1f;
        }
    }
}
