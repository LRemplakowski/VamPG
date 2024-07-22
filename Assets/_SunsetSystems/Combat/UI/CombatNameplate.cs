using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Inventory;
using SunsetSystems.Utils.ObjectPooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Combat.UI
{
    public class CombatNameplate : SerializedMonoBehaviour, IPooledObject
    {
        [Title("References")]
        [field: SerializeField, Required]
        public RectTransform NameplateRect { get; private set; }
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
        private Dictionary<WeaponType, Sprite> _weaponTypeIcons = new();

        private void OnValidate()
        {
            foreach (WeaponType weaponType in Enum.GetValues(typeof(WeaponType)))
            {
                _weaponTypeIcons.TryAdd(weaponType, null);
            }
        }

        public void UpdateNameplateData(CombatNameplateData nameplateData)
        {
            _name.text = nameplateData.Name;
            _healthValue.text = $"{nameplateData.CurrentHP}/{nameplateData.MaxHP}";
            _healthSlider.value = nameplateData.HealthPercentage;
            if (_weaponTypeIcons.TryGetValue(nameplateData.CurrentWeapon, out var icon))
                _enemyTypeImage.sprite = icon;
        }

        public void ResetObject()
        {
            _name.text = "";
            _healthValue.text = "";
            _healthSlider.value = 1f;
        }
    }
}
