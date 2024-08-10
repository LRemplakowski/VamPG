using SunsetSystems.Core.Database;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Inventory.Data
{
    public interface IBaseItem : IDatabaseEntry<IBaseItem>
    {
        string Name { get; }
        bool Stackable { get; }
        ItemCategory ItemCategory { get; }
        Sprite Icon { get; }
        AssetReferenceGameObject WorldSpaceRepresentation { get; }
    }
}
