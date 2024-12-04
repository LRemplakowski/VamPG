using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Equipment;
using UnityEngine;

namespace SunsetSystems.Combat.UI
{
    public class ActionBarUI : SerializedMonoBehaviour
    {
        [SerializeField]
        private CombatActionType _defaultActions;
        [SerializeField]
        private Dictionary<CombatActionType, GameObject> _basicActions = new();

        public void RefreshAvailableActions()
        {
            var actionFlags = GetAvailableActions();
            foreach (var key in _basicActions.Keys)
            {
                _basicActions[key].SetActive(actionFlags.HasFlag(key));
            }
        }

        private CombatActionType GetAvailableActions()
        {
            CombatActionType result = CombatActionType.None;
            //result |= _defaultActions;
            //result |= GetWeaponActions();
            return result;

            //static CombatActionType GetWeaponActions()
            //{
            //    var combatant = CombatManager.Instance.CurrentActiveActor;
            //    var weapon = combatant.References.GetCachedComponentInChildren<IWeaponManager>().GetSelectedWeapon();
            //    return weapon.GetWeaponCombatActions();
            //}
        }
    }
}