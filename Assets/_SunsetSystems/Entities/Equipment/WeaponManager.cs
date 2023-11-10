using Sirenix.OdinInspector;
using SunsetSystems.Animation;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Equipment
{
    public class WeaponManager : SerializedMonoBehaviour, IWeaponManager
    {
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
        private GameObject weaponInstance;

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

        private async Task<GameObject> InstantiateCurrentWeapon()
        {
            if (selectedWeapon != EquipmentSlotID.Invalid)
                return await Addressables.InstantiateAsync(GetSelectedWeapon().EquippedInstanceAsset, weaponParent).Task;
            else
                return null;
        }

        private void ReleaseCurrentWeaponInstance()
        {
            Addressables.ReleaseInstance(weaponInstance);
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
    }
}
