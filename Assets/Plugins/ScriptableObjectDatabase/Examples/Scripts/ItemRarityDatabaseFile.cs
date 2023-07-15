//Created by Scriptable Object Database plugin.

using MyLib.Shared.Database;

[System.Serializable]
public class ItemRarityDatabaseFile : DatabaseFileBase<ItemRarityDatabaseClass, ItemRarityDatabaseAsset>
{
    public ItemRarityDatabaseFile(string name, short id16) : base(name, id16) { }

    public override int AddNew(string name, UnityEngine.Object value)
    {
        return AddNew(new ItemRarityDatabaseAsset(name, data.ID16, data.UniqueAssetKey, value as ItemRaritySObject));
    }
}

[System.Serializable]
public class ItemRarityDatabaseClass : Database<ItemRarityDatabaseAsset> { };

[System.Serializable]
public class ItemRarityDatabaseAsset : DatabaseAsset<ItemRaritySObject>
{
    public ItemRarityDatabaseAsset(string name, short databaseId16, short assetId, ItemRaritySObject objectReference)
        : base(name, databaseId16, assetId, objectReference) { }
}