using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Inventory.Data
{
    public interface IBaseItem
    {
        string Name { get; }
        string ReadableID { get; }
        string DatabaseID { get; }
        bool Stackable { get; }
        ItemCategory ItemCategory { get; }
        Sprite Icon { get; }
        AssetReferenceGameObject WorldSpaceRepresentation { get; }
    }
}
