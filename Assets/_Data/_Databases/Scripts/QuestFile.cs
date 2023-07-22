//Created by Scriptable Object Database plugin.
using SunsetSystems.Journal;
using MyLib.Shared.Database;

[System.Serializable]
public class QuestFile : DatabaseFileBase<QuestClass, QuestAsset>
{
    public QuestFile(string name, short id16) : base(name, id16) { }

    public override int AddNew(string name, UnityEngine.Object value)
    {
        return AddNew(new QuestAsset(name, data.ID16, data.UniqueAssetKey, value as Quest));
    }
}

[System.Serializable]
public class QuestClass : Database<QuestAsset> { };

[System.Serializable]
public class QuestAsset : DatabaseAsset<Quest>
{
    public QuestAsset(string name, short databaseId16, short assetId, Quest objectReference)
        : base(name, databaseId16, assetId, objectReference) { }
}