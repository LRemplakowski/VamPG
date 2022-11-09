using SunsetSystems.UI.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Inventory.UI
{
    public class EquipmentContentsUpdater : MonoBehaviour, IUserInterfaceUpdateReciever<EquipmentSlot>
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
            
        }

        public void UpdateViews(IList<IGameDataProvider<EquipmentSlot>> data)
        {
            foreach (IGameDataProvider<EquipmentSlot> slot in data)
            {
                EquipmentSlotDisplay view = _slotViews[slot.Data.ID];
                view.UpdateView(slot);
                view.gameObject.SetActive(true);
            }
        }
    }
}
