using SunsetSystems.Inventory;

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
        None = 0, Primary = 1, Secondary = 2
    }
}
