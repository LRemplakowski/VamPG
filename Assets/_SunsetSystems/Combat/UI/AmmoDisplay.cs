using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Equipment;
using TMPro;
using UnityEngine;

namespace SunsetSystems.Combat.UI
{
    public class AmmoDisplay : SerializedMonoBehaviour
    {
        [SerializeField, Required]
        private TextMeshProUGUI _ammoText;

        public void SetAmmoCounterVisible(bool visible)
        {
            _ammoText.enabled = visible;
        }

        public void UpdateAmmoData(WeaponAmmoData ammoData)
        {
            _ammoText.text = $"{ammoData.CurrentAmmo}/{ammoData.MaxAmmo}";
        }
    }
}
