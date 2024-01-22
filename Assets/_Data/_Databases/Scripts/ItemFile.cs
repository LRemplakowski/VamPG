//Created by Scriptable Object Database plugin.
using MyLib.Shared.Database;
using SunsetSystems.Inventory.Data;

[System.Serializable]
public class ItemFile : DatabaseFileBase<ItemClass, ItemAsset>
{
    public ItemFile(string name, short id16) : base(name, id16) { }

    public override int AddNew(string name, UnityEngine.Object value)
    {
        return AddNew(new ItemAsset(name, data.ID16, data.UniqueAssetKey, value as BaseItem));
    }
}

[System.Serializable]
public class ItemClass : Database<ItemAsset> { };

[System.Serializable]
public class ItemAsset : DatabaseAsset<BaseItem>
{
    public ItemAsset(string name, short databaseId16, short assetId, BaseItem objectReference)
        : base(name, databaseId16, assetId, objectReference) { }
}