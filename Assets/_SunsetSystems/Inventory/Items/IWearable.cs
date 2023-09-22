using System.Collections;
using System.Collections.Generic;
using UMA;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    public interface IWearable : IEquipableItem
    {
        List<UMARecipeBase> WornRecipes { get; }
    }
}
