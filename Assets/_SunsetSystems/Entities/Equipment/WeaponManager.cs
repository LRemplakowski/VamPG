using Sirenix.OdinInspector;
using SunsetSystems.Animation;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Equipment
{
    public class WeaponManager : SerializedMonoBehaviour, IWeaponManager
    {
        private const string FIRE_WEAPON_EVENT = "FIRE_WEAPON";

        [Title("References")]
        [SerializeField, Required]
        private IEquipmentManager equipmentManager;
        [SerializeField, Required]
        private Transform weaponParent;
        [SerializeField, Required]
        private CreatureAnimationController animationController;
        [Title("Config")]
        [SerializeField]
        private string weaponAnimationTypeParam;
        private int weaponAnimationTypeParamHash;
        [Title("Debug Info")]
        [ShowInInspector, ReadOnly]
        private EquipmentSlotID selectedWeapon = EquipmentSlotID.PrimaryWeapon;
        [SerializeField, ReadOnly]
        private IWeaponInstance weaponInstance;

        private void Start()
        {
            weaponAnimationTypeParamHash = Animator.StringToHash(weaponAnimationTypeParam);
            SetSelectedWeapon(SelectedWeapon.None);
        }

        [Button]
        public void SetSelectedWeapon(SelectedWeapon weapon)
        {
            EquipmentSlotID newSelectedWeapon = weapon switch
            {
                SelectedWeapon.Primary => EquipmentSlotID.PrimaryWeapon,
                SelectedWeapon.Secondary => EquipmentSlotID.SecondaryWeapon,
                SelectedWeapon.None => EquipmentSlotID.Invalid,
                _ => EquipmentSlotID.PrimaryWeapon,
            };
            if (newSelectedWeapon != selectedWeapon)
            {
                selectedWeapon = newSelectedWeapon;
                _ = RebuildWeaponInstance();
            }
            if (weapon != SelectedWeapon.None)
                animationController.SetInteger(weaponAnimationTypeParamHash, (int)(GetSelectedWeapon()?.WeaponType ?? 0));
        }

        private async Task RebuildWeaponInstance()
        {
            if (weaponInstance != null)
                ReleaseCurrentWeaponInstance();
            weaponInstance = await InstantiateCurrentWeapon();
        }

        private async Task<IWeaponInstance> InstantiateCurrentWeapon()
        {
            if (selectedWeapon != EquipmentSlotID.Invalid)
                return (await Addressables.InstantiateAsync(GetSelectedWeapon().EquippedInstanceAsset, weaponParent).Task).GetComponent<IWeaponInstance>();
            else
                return null;
        }

        private void ReleaseCurrentWeaponInstance()
        {
            Addressables.ReleaseInstance(weaponInstance.GameObject);
        }

        public IWeapon GetSelectedWeapon()
        {
            return equipmentManager.EquipmentSlots[selectedWeapon].GetEquippedItem() as IWeapon;
        }

        public IWeapon GetPrimaryWeapon()
        {
            return equipmentManager.EquipmentSlots[EquipmentSlotID.PrimaryWeapon].GetEquippedItem() as IWeapon;
        }

        public IWeapon GetSecondaryWeapon()
        {
            return equipmentManager.EquipmentSlots[EquipmentSlotID.SecondaryWeapon].GetEquippedItem() as IWeapon;
        }

        public void OnItemEquipped(IEquipableItem item)
        {
            if (item is IWeapon weapon)
            {
                switch (selectedWeapon)
                {
                    case EquipmentSlotID.PrimaryWeapon:
                        if (GetPrimaryWeapon() == weapon)
                            _ = RebuildWeaponInstance();
                        break;
                    case EquipmentSlotID.SecondaryWeapon:
                        if (GetSecondaryWeapon() == weapon)
                            _ = RebuildWeaponInstance();
                        break;
                }
            }
        }

        public void OnItemUnequipped(IEquipableItem item)
        {
            if (item is IWeapon)
            {
                switch (selectedWeapon)
                {
                    case EquipmentSlotID.PrimaryWeapon:
                        if (GetPrimaryWeapon() == null && weaponInstance != null)
                            ReleaseCurrentWeaponInstance();
                        break;
                    case EquipmentSlotID.SecondaryWeapon:
                        if (GetSecondaryWeapon() == null && weaponInstance != null)
                            ReleaseCurrentWeaponInstance();
                        break;
                }
            }
        }

        public void OnAnimationEvent(string eventType)
        {
            if (string.Equals(eventType, FIRE_WEAPON_EVENT))
                weaponInstance.FireWeapon();
        }
    }
}
