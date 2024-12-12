using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Abilities;
using UltEvents;
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
        [Title("Events")]
        [InlineProperty]
        public UltEvent<IAbility> OnAbilitySelected;

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

        private void SelectAbility(IAbility ability)
        {
            OnAbilitySelected?.InvokeSafe(ability);
        }

        private IEnumerable<IAbility> GetCoreAbilities()
        {
            return CombatManager.Instance.CurrentActiveActor.GetContext().AbilityUser.GetCoreAbilities();
        }
    }
}