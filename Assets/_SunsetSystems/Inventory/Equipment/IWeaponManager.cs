using SunsetSystems.Combat;
using SunsetSystems.Inventory;
using UltEvents;

namespace SunsetSystems.Equipment
{
    public interface IWeaponManager
    {
        UltEvent<ICombatant> OnWeaponChanged { get; set; }

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
