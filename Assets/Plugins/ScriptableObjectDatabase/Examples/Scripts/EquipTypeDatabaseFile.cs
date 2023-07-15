//Created by Scriptable Object Database plugin.

using MyLib.Shared.Database;

[System.Serializable]
public class EquipTypeDatabaseFile : DatabaseFileBase<EquipTypeDatabaseClass, EquipTypeDatabaseAsset>
{
    public EquipTypeDatabaseFile(string name, short id16) : base(name, id16) { }

    public override int AddNew(string name, UnityEngine.Object value)
    {
        return AddNew(new EquipTypeDatabaseAsset(name, data.ID16, data.UniqueAssetKey, value as EquippableItemTypeSObject));
    }
}

[System.Serializable]
public class EquipTypeDatabaseClass : Database<EquipTypeDatabaseAsset> { };

[System.Serializable]
public class EquipTypeDatabaseAsset : DatabaseAsset<EquippableItemTypeSObject>
{
    public EquipTypeDatabaseAsset(string name, short databaseId16, short assetId, EquippableItemTypeSObject objectReference)
        : base(name, databaseId16, assetId, objectReference) { }
}