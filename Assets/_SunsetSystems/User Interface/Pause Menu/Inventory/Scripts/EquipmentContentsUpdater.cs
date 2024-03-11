using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.UI.Utils;
using UnityEngine;

namespace SunsetSystems.Equipment.UI
{
    public class EquipmentContentsUpdater : SerializedMonoBehaviour, IUserInterfaceUpdateReciever<IEquipmentSlot>
    {
        [SerializeField]
        private EquipmentSlotDisplay _slotPrefab;

        [SerializeField, DictionaryDrawerSettings(IsReadOnly = true, KeyLabel = "Slot ID", ValueLabel = "Slot View")]
        private Dictionary<EquipmentSlotID, EquipmentSlotDisplay> _slotViews = new();

        private void OnValidate()
        {
            if (_slotViews == null)
                _slotViews = new();
            foreach (EquipmentSlotID key in EquipmentData.GetSlotsPreset().Keys)
            {
                _slotViews.TryAdd(key, null);
            }
        }

        public void DisableViews()
        {
            
        }

        public void UpdateViews(List<IGameDataProvider<IEquipmentSlot>> data)
        {
            foreach (IGameDataProvider<IEquipmentSlot> slot in data)
            {
                if (_slotViews.TryGetValue(slot.Data.ID, out EquipmentSlotDisplay view))
                {
                    view.UpdateView(slot);
                    view.gameObject.SetActive(true);
                }
            }
        }
    }
}
