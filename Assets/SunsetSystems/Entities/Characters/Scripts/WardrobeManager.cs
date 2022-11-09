using SunsetSystems.Inventory;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    public class WardrobeManager : MonoBehaviour
    {
        private Transform _rightHand, _leftHand;
        private DynamicCharacterAvatar _dca;
        private string _characterID;
        private WardrobeData _cachedWardrobeData = new();
        private bool _initializedOnce;

        private void OnEnable()
        {
            InventoryManager.ItemEquipped += UpdateWardrobe;
            InventoryManager.ItemUnequipped += UpdateWardrobe;
        }

        private void OnDisable()
        {
            InventoryManager.ItemEquipped -= UpdateWardrobe;
            InventoryManager.ItemUnequipped -= UpdateWardrobe;
        }

        public void UpdateWardrobe(string characterID)
        {
            if (!_initializedOnce)
            {
                UMAData data = GetComponent<UMAData>();
                _rightHand = data.GetBoneGameObject("RightHand").transform;
                _leftHand = data.GetBoneGameObject("LeftHand").transform;
                _dca = GetComponent<DynamicCharacterAvatar>();
                _characterID = GetComponent<Creature>().Data.ID;
                _initializedOnce = true;
            }
            if (this._characterID.Equals(characterID))
            {
                if (InventoryManager.TryGetEquipmentData(characterID, out EquipmentData data))
                {
                    RemoveWardrobe();
                    GameObject weapon = Instantiate(data.equipmentSlots[EquipmentData.SLOT_WEAPON_PRIMARY].GetEquippedItem().Prefab, _rightHand);
                    _cachedWardrobeData.CurrentWeapon = weapon;
                }                
            }
        }

        private void RemoveWardrobe()
        {
            Destroy(_cachedWardrobeData.CurrentWeapon);
        }

        private struct WardrobeData
        {
            public GameObject CurrentWeapon;
        }
    }
}
