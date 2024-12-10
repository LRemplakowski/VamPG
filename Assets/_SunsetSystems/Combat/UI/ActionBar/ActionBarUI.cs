using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Abilities;
using SunsetSystems.Equipment;
using UnityEngine;

namespace SunsetSystems.Combat.UI
{
    public class ActionBarUI : SerializedMonoBehaviour
    {
        [SerializeField, AssetsOnly]
        private CombatActionSelectorButton _abilityButtonPrefab;
        [SerializeField]
        private Dictionary<IAbility, GameObject> _basicActions = new();

        public void RefreshAvailableActions()
        {
            
        }

        private IEnumerable<IAbility> GetAvailableActions()
        {
            return CombatManager.Instance.CurrentActiveActor.GetContext().AbilityUser.GetCoreAbilities();
        }
    }
}