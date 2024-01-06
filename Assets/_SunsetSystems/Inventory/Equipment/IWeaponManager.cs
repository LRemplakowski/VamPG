using SunsetSystems.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Equipment
{
    public interface IWeaponManager
    {
        void SetSelectedWeapon(SelectedWeapon weapon);
        IWeapon GetSelectedWeapon();
        IWeapon GetPrimaryWeapon();
        IWeapon GetSecondaryWeapon();

        bool UseAmmoFromSelectedWeapon(int count);
        void ReloadSelectedWeapon();
    }

    public enum SelectedWeapon
    {
        None = 1, Primary = 2, Secondary = 3
    }
}
