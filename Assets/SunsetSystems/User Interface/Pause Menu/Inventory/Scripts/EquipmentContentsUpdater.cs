using SunsetSystems.UI.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory.UI
{
    public class EquipmentContentsUpdater : MonoBehaviour, IUserInterfaceUpdateReciever<EquipmentSlot, EquipmentSlotDisplay>
    {
        [SerializeField]
        private EquipmentSlotDisplay _slotPrefab;

        [SerializeField]
        private StringEquipmentSlotDisplayDictionary _slotViews;

        private void OnValidate()
        {
            StringEquipmentSlotDictionary slotPairs = EquipmentData.Initialize().equipmentSlots;
            foreach (string key in slotPairs.Keys)
            {
                _slotViews.TryAdd(key, null);
            }
        }

        public void DisableViews()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateViews(IList<IGameDataProvider<EquipmentSlot>> data)
        {
            throw new System.NotImplementedException();
        }
    }
}
