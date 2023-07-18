//Created by Scriptable Object Database plugin.
using SunsetSystems.Journal;
using MyLib.Shared.Database;

[System.Serializable]
public class ObjectiveFile : DatabaseFileBase<ObjectiveClass, ObjectiveAsset>
{
    public ObjectiveFile(string name, short id16) : base(name, id16) { }

    public override int AddNew(string name, UnityEngine.Object value)
    {
        return AddNew(new ObjectiveAsset(name, data.ID16, data.UniqueAssetKey, value as Objective));
    }
}

[System.Serializable]
public class ObjectiveClass : Database<ObjectiveAsset> { };

[System.Serializable]
public class ObjectiveAsset : DatabaseAsset<Objective>
{
    public ObjectiveAsset(string name, short databaseId16, short assetId, Objective objectReference)
        : base(name, databaseId16, assetId, objectReference) { }
}