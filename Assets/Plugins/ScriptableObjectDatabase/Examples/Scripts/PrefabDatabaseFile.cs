using System;
using MyLib.Shared.Database;
using UnityEngine;

[System.Serializable]
public class PrefabDatabaseFile : DatabaseFileBase<PrefabDatabaseClass, PrefabDatabaseAsset>
{
    public PrefabDatabaseFile(string name, short id16) : base(name, id16) { }

    public override int AddNew(string name, UnityEngine.Object value)
    {
        return AddNew(new PrefabDatabaseAsset(name, data.ID16, data.UniqueAssetKey, value as GameObject));
    }
}

[System.Serializable]
public class PrefabDatabaseClass : Database<PrefabDatabaseAsset> { };

[System.Serializable]
public class PrefabDatabaseAsset : DatabaseAsset<GameObject>
{
    public PrefabDatabaseAsset(string name, short databaseId16, short assetId, GameObject gameObject)
        : base(name, databaseId16, assetId, gameObject) { }
}