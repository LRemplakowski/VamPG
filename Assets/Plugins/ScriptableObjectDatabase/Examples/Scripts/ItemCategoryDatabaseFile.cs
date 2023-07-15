//Created by Scriptable Object Database plugin.

using MyLib.Shared.Database;

[System.Serializable]
public class ItemCategoryDatabaseFile : DatabaseFileBase<ItemCategoryDatabaseClass, ItemCategoryDatabaseAsset>
{
    public ItemCategoryDatabaseFile(string name, short id16) : base(name, id16) { }

    public override int AddNew(string name, UnityEngine.Object value)
    {
        return AddNew(new ItemCategoryDatabaseAsset(name, data.ID16, data.UniqueAssetKey, value as ItemCategorySObject));
    }
}

[System.Serializable]
public class ItemCategoryDatabaseClass : Database<ItemCategoryDatabaseAsset> { };

[System.Serializable]
public class ItemCategoryDatabaseAsset : DatabaseAsset<ItemCategorySObject>
{
    public ItemCategoryDatabaseAsset(string name, short databaseId16, short assetId, ItemCategorySObject objectReference)
        : base(name, databaseId16, assetId, objectReference) { }
}