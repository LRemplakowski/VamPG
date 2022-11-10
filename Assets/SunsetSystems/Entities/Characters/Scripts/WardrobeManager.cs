using SunsetSystems.Animation;
using SunsetSystems.Game;
using SunsetSystems.Inventory;
using System;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace SunsetSystems.Entities.Characters
{
    public class WardrobeManager : MonoBehaviour
    {
        private Creature _owner;
        private CreatureAnimationController _animationController;
        private Transform _rightClavicle, _leftHand;
        private DynamicCharacterAvatar _dca;
        private string _characterID;
        private WardrobeData _cachedWardrobeData = new();
        private bool _initializedOnce;
        private bool _displayWeapons = false;

        private void OnEnable()
        {
            InventoryManager.ItemEquipped += UpdateWardrobe;
            InventoryManager.ItemUnequipped += UpdateWardrobe;
            GameManager.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnDisable()
        {
            InventoryManager.ItemEquipped -= UpdateWardrobe;
            InventoryManager.ItemUnequipped -= UpdateWardrobe;
            GameManager.OnGameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState currentState)
        {
            if (currentState.Equals(GameState.Combat))
                _displayWeapons = true;
            else
                _displayWeapons = false;
            if (!_initializedOnce)
                Initialize();
            UpdateWardrobe(_characterID);
        }

        private void Initialize()
        {
            UMAData data = GetComponent<UMAData>();
            _rightClavicle = data.GetBoneGameObject("CC_Base_R_Clavicle").transform;
            _dca = GetComponent<DynamicCharacterAvatar>();
            _owner = GetComponent<Creature>();
            _characterID = GetComponent<Creature>().Data.ID;
            _initializedOnce = true;
            _animationController = GetComponent<CreatureAnimationController>();
        }    

        public void UpdateWardrobe(string characterID)
        {
            if (!_initializedOnce)
                Initialize();
            if (this._characterID.Equals(characterID))
            {
                EquipmentData data = _owner.Data.equipment;
                RemoveWardrobe();
                HandleWeapon(data);                
            }
        }

        private void HandleWeapon(EquipmentData data)
        {
            if (GameManager.IsCurrentState(GameState.Combat) == false)
            {
                _animationController.DisableIK();
                return;
            }

            EquipmentSlot slotData = data.equipmentSlots[EquipmentData.SLOT_WEAPON_PRIMARY];
            if (slotData.GetEquippedItem() == null)
            {
                _animationController.DisableIK();
                return;
            }

            GameObject weapon = Instantiate(slotData.GetEquippedItem().Prefab, _rightClavicle);
            _cachedWardrobeData.CurrentWeapon = weapon;
            if (weapon.TryGetComponent(out AnimationDataProvider ikData))
                _animationController.EnableIK(ikData);

        }

        private void RemoveWardrobe()
        {
            if (_cachedWardrobeData.CurrentWeapon != null)
                Destroy(_cachedWardrobeData.CurrentWeapon);
        }

        private struct WardrobeData
        {
            public GameObject CurrentWeapon;
        }
    }
}
