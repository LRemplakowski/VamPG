//Created by Scriptable Object Database plugin.
using MyLib.Shared.Database;
using UMA.CharacterSystem;

[System.Serializable]
public class WardrobeCollectionDatabaseFile : DatabaseFileBase<WardrobeCollectionDatabaseClass, WardrobeCollectionDatabaseAsset>
{
    public WardrobeCollectionDatabaseFile(string name, short id16) : base(name, id16) { }

    public override int AddNew(string name, UnityEngine.Object value)
    {
        return AddNew(new WardrobeCollectionDatabaseAsset(name, data.ID16, data.UniqueAssetKey, value as UMAWardrobeCollection));
    }
}

[System.Serializable]
public class WardrobeCollectionDatabaseClass : Database<WardrobeCollectionDatabaseAsset> { };

[System.Serializable]
public class WardrobeCollectionDatabaseAsset : DatabaseAsset<UMAWardrobeCollection>
{
    public WardrobeCollectionDatabaseAsset(string name, short databaseId16, short assetId, UMAWardrobeCollection objectReference)
        : base(name, databaseId16, assetId, objectReference) { }
}