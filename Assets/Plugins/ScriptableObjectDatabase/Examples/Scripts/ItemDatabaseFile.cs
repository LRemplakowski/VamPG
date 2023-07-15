//Created by Scriptable Object Database plugin.
using System;
using MyLib.Shared.Database;
using UnityEngine;

[System.Serializable]
public class ItemDatabaseFile : DatabaseFileBase<ItemDatabaseClass, ItemDatabaseAsset>
{
    public ItemDatabaseFile(string name, short id16) : base(name, id16) { }

    public override int AddNew(string name, UnityEngine.Object value)
    {
        return AddNew(new ItemDatabaseAsset(name, data.ID16, data.UniqueAssetKey, value as ItemBaseSObject));
    }
}

[System.Serializable]
public class ItemDatabaseClass : Database<ItemDatabaseAsset> { };

[System.Serializable]
public class ItemDatabaseAsset : DatabaseAsset<ItemBaseSObject>
{
    public ItemDatabaseAsset(string name, short databaseId16, short assetId, ItemBaseSObject objectReference)
        : base(name, databaseId16, assetId, objectReference) { }
}