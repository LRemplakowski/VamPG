using Sirenix.OdinInspector;
using SunsetSystems.Equipment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Combat.UI
{
    public class WeaponSetSelectorButton : SerializedMonoBehaviour
    {
        [SerializeField]
        private SelectedWeapon associatedWeapon;

        public static event Action<SelectedWeapon> OnWeaponSelected;

        public void OnClick()
        {
            OnWeaponSelected?.Invoke(associatedWeapon);
        }
    }
}
