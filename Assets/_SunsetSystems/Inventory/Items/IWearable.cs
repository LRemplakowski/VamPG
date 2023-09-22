using System.Collections;
using System.Collections.Generic;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    public interface IWearable : IEquipableItem
    {
        UMAWardrobeCollection WearableWardrobe { get; }
    }
}
