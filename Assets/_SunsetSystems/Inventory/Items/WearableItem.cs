using System.Collections.Generic;
using UMA;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    public abstract class WearableItem : EquipableItem, IWearable
    {
        [field: SerializeField]
        public List<UMARecipeBase> WornRecipes { get; private set; } = new();
    }
}
