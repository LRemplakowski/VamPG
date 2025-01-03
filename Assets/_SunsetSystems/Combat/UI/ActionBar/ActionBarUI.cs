using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Abilities;
using SunsetSystems.Equipment;
using UnityEngine;

namespace SunsetSystems.Combat.UI
{
    public class ActionBarUI : SerializedMonoBehaviour
    {
        [Title("Config")]
        [SerializeField]
        private Transform _coreButtonsParent;
        [SerializeField, AssetsOnly]
        private IAbilityButtonFactory _buttonFactory;

        private readonly Dictionary<IAbilityConfig, Action<WeaponAmmoData>> _ammoUpdatesMap = new();

        private IAbilityConfig _cachedLastSelectedAbility;
        public static event Action<IAbilityConfig> OnAbilitySelected;

        public void RefreshAvailableActions()
        {
            RefreshCoreAbilities();
        }

        public void UpdateAmmoCounter(in WeaponAmmoData ammoData)
        {
            if (_cachedLastSelectedAbility != null && _ammoUpdatesMap.TryGetValue(_cachedLastSelectedAbility, out var onAmmoUpdate))
            {
                onAmmoUpdate?.Invoke(ammoData);
            }
        }

        private void RefreshCoreAbilities()
        {
            _ammoUpdatesMap.Clear();
            _coreButtonsParent.DestroyChildren();
            foreach (var ability in GetCoreAbilities())
            {
                _buttonFactory.Create(_coreButtonsParent, ability, SelectAbility, out var onAmmoUpdate);
                _ammoUpdatesMap.TryAdd(ability, onAmmoUpdate);
            }
        }

        private void SelectAbility(IAbilityConfig ability)
        {
            _cachedLastSelectedAbility = ability;
            OnAbilitySelected?.Invoke(ability);
        }

        private IEnumerable<IAbilityConfig> GetCoreAbilities()
        {
            return CombatManager.Instance.CurrentActiveActor.GetContext().AbilityUser.GetCoreAbilities();
        }
    }
}