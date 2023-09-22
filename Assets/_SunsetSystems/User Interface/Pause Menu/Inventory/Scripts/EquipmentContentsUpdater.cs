using SunsetSystems.Entities.Characters;
using SunsetSystems.UI.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Equipment.UI
{
    public class EquipmentContentsUpdater : MonoBehaviour, IUserInterfaceUpdateReciever<IEquipmentSlot>
    {
        [SerializeField]
        private EquipmentSlotDisplay _slotPrefab;

        [SerializeField]
        private Dictionary<EquipmentSlotID, EquipmentSlotDisplay> _slotViews;

        private void OnValidate()
        {
            foreach (EquipmentSlotID key in EquipmentData.GetSlotsPreset().Keys)
            {
                _slotViews?.TryAdd(key, null);
            }
        }

        public void DisableViews()
        {
            
        }

        public void UpdateViews(List<IGameDataProvider<IEquipmentSlot>> data)
        {
            foreach (IGameDataProvider<IEquipmentSlot> slot in data)
            {
                EquipmentSlotDisplay view = _slotViews[slot.Data.ID];
                view.UpdateView(slot);
                view.gameObject.SetActive(true);
            }
        }
    }
}
