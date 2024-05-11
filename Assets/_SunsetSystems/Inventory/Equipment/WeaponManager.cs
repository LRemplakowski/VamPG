using Sirenix.OdinInspector;
using SunsetSystems.Animation;
using SunsetSystems.Combat;
using SunsetSystems.Combat.UI;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Game;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using System.Collections.Generic;
using System.Linq;
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
        private AnimationManager animationController;
        [SerializeField, Required]
        private ICombatant owner;
        [Title("Config")]
        [SerializeField]
        private string weaponAnimationTypeParam;
        private int weaponAnimationTypeParamHash;
        [SerializeField]
        private bool _ignoreAmmo;
        [SerializeField]
        private bool _showWeaponOutsideCombat;
        [Title("Debug Info")]
        [SerializeField, ReadOnly]
        private EquipmentSlotID selectedWeapon = EquipmentSlotID.PrimaryWeapon;
        [ShowInInspector, ReadOnly]
        private IWeaponInstance weaponInstance;
        [ShowInInspector, ReadOnly]
        private Dictionary<EquipmentSlotID, WeaponAmmoData> weaponsAmmoData = new();

        private void OnEnable()
        {
            WeaponSetSelectorButton.OnWeaponSelected += OnWeaponSelected;
            CombatManager.Instance.CombatBegin += OnCombatStart;
            CombatManager.Instance.CombatEnd += OnCombatEnd;
        }

        private void OnDisable()
        {
            WeaponSetSelectorButton.OnWeaponSelected -= OnWeaponSelected;
            CombatManager.Instance.CombatBegin -= OnCombatStart;
            CombatManager.Instance.CombatEnd -= OnCombatEnd;
        }

        private void Start()
        {
            weaponAnimationTypeParamHash = Animator.StringToHash(weaponAnimationTypeParam);
            //SetSelectedWeapon(SelectedWeapon.None);
            weaponsAmmoData ??= new();
            if (_showWeaponOutsideCombat)
                OnCombatStart(new List<ICombatant>() { owner });
        }

        private void OnWeaponSelected(SelectedWeapon weapon)
        {
            SetSelectedWeapon(weapon);
        }

        private void OnCombatStart(IEnumerable<ICombatant> combatants)
        {
            weaponsAmmoData.Clear();
            if (combatants.Contains(owner))
            {
                IWeapon primaryWeapon = GetPrimaryWeapon();
                if (primaryWeapon != null && primaryWeapon.WeaponType == WeaponType.Ranged)
                {
                    WeaponAmmoData primaryWeaponAmmoData = new()
                    {
                        MaxAmmo = primaryWeapon.MaxAmmo,
                        CurrentAmmo = primaryWeapon.MaxAmmo
                    };
                    weaponsAmmoData[EquipmentSlotID.PrimaryWeapon] = primaryWeaponAmmoData;
                }
                IWeapon secondaryWeapon = GetSecondaryWeapon();
                if (secondaryWeapon != null && secondaryWeapon.WeaponType == WeaponType.Ranged)
                {
                    WeaponAmmoData secondaryWeaponAmmoData = new()
                    {
                        MaxAmmo = primaryWeapon.MaxAmmo,
                        CurrentAmmo = primaryWeapon.MaxAmmo
                    };
                    weaponsAmmoData[EquipmentSlotID.SecondaryWeapon] = secondaryWeaponAmmoData;
                }
                _ = RebuildWeaponInstance();
            }
        }

        public void OnCombatEnd()
        {
            ReleaseCurrentWeaponInstance();
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
#if UNITY_EDITOR
                if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
#endif
                    await RebuildWeaponInstance();
            }
        }

        private async Task RebuildWeaponInstance()
        {
            if (weaponInstance != null)
                ReleaseCurrentWeaponInstance();
            if (GameManager.Instance.IsCurrentState(GameState.Combat) is false && _showWeaponOutsideCombat is false)
                return;
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
            if (weaponInstance != null)
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
            IWeapon selectedWeaponInstance = GetSelectedWeapon();
            if (selectedWeaponInstance == null || selectedWeaponInstance.WeaponType == WeaponType.Melee || _ignoreAmmo)
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
            IWeapon selectedWeaponInstance = GetSelectedWeapon();
            if (selectedWeaponInstance == null || selectedWeaponInstance.WeaponType == WeaponType.Melee || _ignoreAmmo)
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
