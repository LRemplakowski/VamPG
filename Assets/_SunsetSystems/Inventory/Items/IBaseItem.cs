using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Inventory.Data
{
    public interface IBaseItem
    {
        string Name { get; }
        string DatabaseID { get; }
        bool Stackable { get; }
        ItemCategory ItemCategory { get; }
        AssetReferenceSprite Icon { get; }
        AssetReferenceGameObject WorldSpaceRepresentation { get; }
    }
}
