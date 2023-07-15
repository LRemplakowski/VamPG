//Created by Scriptable Object Database plugin.

using MyLib.Shared.Database;

[System.Serializable]
public class EquippableItemDatabaseFile : DatabaseFileBase<EquippableItemDatabaseClass, EquippableItemDatabaseAsset>
{
    public EquippableItemDatabaseFile(string name, short id16) : base(name, id16) { }

    public override int AddNew(string name, UnityEngine.Object value)
    {
        return AddNew(new EquippableItemDatabaseAsset(name, data.ID16, data.UniqueAssetKey, value as ItemBaseSObject));
    }
}

[System.Serializable]
public class EquippableItemDatabaseClass : Database<EquippableItemDatabaseAsset> { };

[System.Serializable]
public class EquippableItemDatabaseAsset : DatabaseAsset<ItemBaseSObject>
{
    public EquippableItemDatabaseAsset(string name, short databaseId16, short assetId, ItemBaseSObject objectReference)
        : base(name, databaseId16, assetId, objectReference) { }
}