using Sirenix.OdinInspector;
using SunsetSystems.Animation;
using SunsetSystems.Combat;
using SunsetSystems.Combat.UI;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using System.Collections.Generic;
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
        [SerializeField, Required]
        private ICombatant owner;
        [Title("Config")]
        [SerializeField]
        private string weaponAnimationTypeParam;
        private int weaponAnimationTypeParamHash;
        [Title("Debug Info")]
        [ShowInInspector, ReadOnly]
        private EquipmentSlotID selectedWeapon = EquipmentSlotID.PrimaryWeapon;
        [ShowInInspector, ReadOnly]
        private IWeaponInstance weaponInstance;
        [ShowInInspector, ReadOnly]
        private Dictionary<IWeapon, WeaponAmmoData> weaponsAmmoData = new();

        private void OnEnable()
        {
            WeaponSetSelectorButton.OnWeaponSelected += OnWeaponSelected;
            CombatManager.Instance.CombatBegin += OnCombatStart;
        }

        private void OnDisable()
        {
            WeaponSetSelectorButton.OnWeaponSelected -= OnWeaponSelected;
            CombatManager.Instance.CombatBegin -= OnCombatStart;
        }

        private void Start()
        {
            weaponAnimationTypeParamHash = Animator.StringToHash(weaponAnimationTypeParam);
            SetSelectedWeapon(SelectedWeapon.None);
            weaponsAmmoData ??= new();
        }

        private void OnWeaponSelected(SelectedWeapon weapon)
        {
            if (CombatManager.Instance.CurrentActiveActor == owner)
                SetSelectedWeapon(weapon);
        }

        private void OnCombatStart(IEnumerable<ICombatant> combatants)
        {
            weaponsAmmoData.Clear();
            IWeapon primaryWeapon = GetPrimaryWeapon();
            if (primaryWeapon != null && primaryWeapon.WeaponType == WeaponType.Ranged)
            {
                WeaponAmmoData primaryWeaponAmmoData = new()
                {
                    MaxAmmo = primaryWeapon.MaxAmmo,
                    CurrentAmmo = primaryWeapon.MaxAmmo
                };
                weaponsAmmoData.Add(primaryWeapon, primaryWeaponAmmoData);
            }
            IWeapon secondaryWeapon = GetSecondaryWeapon();
            if (secondaryWeapon != null && secondaryWeapon.WeaponType == WeaponType.Ranged)
            {
                WeaponAmmoData secondaryWeaponAmmoData = new()
                {
                    MaxAmmo = primaryWeapon.MaxAmmo,
                    CurrentAmmo = primaryWeapon.MaxAmmo
                };
                weaponsAmmoData.Add(secondaryWeapon, secondaryWeaponAmmoData);
            }
        }

        [Button]
        public async void SetSelectedWeapon(SelectedWeapon weapon)
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
                await RebuildWeaponInstance();
            }
        }

        private async Task RebuildWeaponInstance()
        {
            if (weaponInstance != null)
                ReleaseCurrentWeaponInstance();
            weaponInstance = await InstantiateCurrentWeapon();
            if (weaponInstance != null)
                animationController.SetInteger(weaponAnimationTypeParamHash, (int)(weaponInstance.WeaponAnimationData.AnimationType));
            else
                animationController.SetInteger(weaponAnimationTypeParamHash, (int)WeaponAnimationType.Brawl);
        }

        private async Task<IWeaponInstance> InstantiateCurrentWeapon()
        {
            if (selectedWeapon != EquipmentSlotID.Invalid && GetSelectedWeapon()?.EquippedInstanceAsset != null)
                return (await Addressables.InstantiateAsync(GetSelectedWeapon().EquippedInstanceAsset, weaponParent).Task).GetComponent<IWeaponInstance>();
            else
                return null;
        }

        private void ReleaseCurrentWeaponInstance()
        {
            Addressables.ReleaseInstance(weaponInstance.GameObject);
            weaponInstance = null;
        }

        public IWeapon GetSelectedWeapon()
        {
            if (equipmentManager.EquipmentSlots.TryGetValue(selectedWeapon, out IEquipmentSlot slot))
                return slot.GetEquippedItem() as IWeapon;
            return null;
        }

        public IWeapon GetPrimaryWeapon()
        {
            return equipmentManager.EquipmentSlots[EquipmentSlotID.PrimaryWeapon].GetEquippedItem() as IWeapon;
        }

        public IWeapon GetSecondaryWeapon()
        {
            return equipmentManager.EquipmentSlots[EquipmentSlotID.SecondaryWeapon].GetEquippedItem() as IWeapon;
        }

        public bool UseAmmoFromSelectedWeapon(int count)
        {
            IWeapon selectedWeapon = GetSelectedWeapon();
            if (selectedWeapon == null || selectedWeapon.WeaponType == WeaponType.Melee)
                return true;
            if (weaponsAmmoData.TryGetValue(selectedWeapon, out WeaponAmmoData ammoData))
            {
                if (ammoData.CurrentAmmo < count)
                    return false;
                ammoData.CurrentAmmo -= count;
                weaponsAmmoData[selectedWeapon] = ammoData;
                return true;
            }
            return false;
        }

        public void ReloadSelectedWeapon()
        {
            IWeapon selectedWeapon = GetSelectedWeapon();
            if (selectedWeapon == null || selectedWeapon.WeaponType == WeaponType.Melee)
                return;
            if (weaponsAmmoData.TryGetValue(selectedWeapon, out WeaponAmmoData ammoData))
            {
                ammoData.CurrentAmmo = ammoData.MaxAmmo;
                weaponsAmmoData[selectedWeapon] = ammoData;
            }
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

        private struct WeaponAmmoData
        {
            public int CurrentAmmo, MaxAmmo;
        }
    }
}
