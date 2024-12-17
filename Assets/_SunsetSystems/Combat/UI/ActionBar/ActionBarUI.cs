using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Abilities;
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

        public static event Action<IAbilityConfig> OnAbilitySelected;

        public void RefreshAvailableActions()
        {
            RefreshCoreAbilities();
        }

        private void RefreshCoreAbilities()
        {
            _coreButtonsParent.DestroyChildren();
            foreach (var ability in GetCoreAbilities())
            {
                _buttonFactory.Create(_coreButtonsParent, ability, SelectAbility);
            }
        }

        private void SelectAbility(IAbilityConfig ability)
        {
            OnAbilitySelected?.Invoke(ability);
        }

        private IEnumerable<IAbilityConfig> GetCoreAbilities()
        {
            return CombatManager.Instance.CurrentActiveActor.GetContext().AbilityUser.GetCoreAbilities();
        }
    }
}