using SunsetSystems.Animation;
using SunsetSystems.Combat;
using SunsetSystems.Game;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using System;
using System.Collections.Generic;
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
            CombatManager.CombatBegin += OnCombatBegin;
            CombatManager.CombatEnd += OnCombatEnd;
        }

        private void OnDisable()
        {
            InventoryManager.ItemEquipped -= UpdateWardrobe;
            InventoryManager.ItemUnequipped -= UpdateWardrobe;
            CombatManager.CombatBegin -= OnCombatBegin;
            CombatManager.CombatEnd -= OnCombatEnd;
        }

        private void OnCombatBegin(List<Creature> combatants)
        {
            if (!_initializedOnce)
                Initialize();
            if (combatants.Contains(_owner))
            {
                _displayWeapons = true;
                UpdateWardrobe(_characterID);
            }
            else
            {
                Debug.LogWarning($"{_characterID} is not in combatants list!");
            }
        }

        private void OnCombatEnd()
        {
            if (!_initializedOnce)
                Initialize();
            _displayWeapons = false;
            UpdateWardrobe(_characterID);
        }

        private void Initialize()
        {
            Debug.LogError($"Initializing wardrobe manager for {gameObject.name}");
            _dca = GetComponent<DynamicCharacterAvatar>();
            _owner = GetComponent<Creature>();
            _characterID = _owner.Data.DatabaseID;
            _initializedOnce = true;
            _animationController = GetComponent<CreatureAnimationController>();
        }    

        public void UpdateWardrobe(string characterID)
        {
            if (!_initializedOnce)
                Initialize();
            if (this._characterID.Equals(characterID))
            {
                EquipmentData data = _owner.Data.Equipment;
                RemoveWardrobe();
                HandleWeapon(data);                
            }
        }

        private void HandleWeapon(EquipmentData data)
        {
            if (_displayWeapons == false)
            {
                _animationController.DisableIK();
                return;
            }

            Weapon selectedWeapons = data.GetSelectedWeapon();
            if (selectedWeapons == null)
            {
                Debug.LogWarning($"No weapon selected for {gameObject.name}!");
                _animationController.DisableIK();
                return;
            }
            
            if (_rightClavicle == null)
            {
                UMAData umaData = GetComponent<UMAData>();
                _rightClavicle = umaData.GetBoneGameObject("CC_Base_R_Clavicle").transform;
            }

            Transform weapon = Instantiate(selectedWeapons.Prefab).transform;
            weapon.SetParent(_rightClavicle);
            _cachedWardrobeData.CurrentWeapon = weapon;
            if (weapon.TryGetComponent(out WeaponAnimationDataProvider ikData))
            {
                Debug.Log($"Enabling weapon IK for {gameObject.name}!");
                _animationController.EnableIK(ikData);
                weapon.localPosition = ikData.PositionOffset;
                weapon.localEulerAngles = ikData.RotationOffset;
            }
        }

        private void RemoveWardrobe()
        {
            if (_cachedWardrobeData.CurrentWeapon != null)
                Destroy(_cachedWardrobeData.CurrentWeapon.gameObject);
        }

        private struct WardrobeData
        {
            public Transform CurrentWeapon;
        }
    }
}
