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
    }

    public enum SelectedWeapon
    {
        None, Primary, Secondary
    }
}
