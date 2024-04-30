using UMA.CharacterSystem;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    public abstract class WearableItem : EquipableItem, IWearable
    {
        [field: SerializeField]
        public UMAWardrobeCollection WearableWardrobe { get; private set; }
    }
}
