//Created by Scriptable Object Database plugin.
using SunsetSystems.Entities.Characters;
using MyLib.Shared.Database;

[System.Serializable]
public class CreatureConfigFile : DatabaseFileBase<CreatureConfigClass, CreatureConfigAsset>
{
    public CreatureConfigFile(string name, short id16) : base(name, id16) { }

    public override int AddNew(string name, UnityEngine.Object value)
    {
        return AddNew(new CreatureConfigAsset(name, data.ID16, data.UniqueAssetKey, value as CreatureConfig));
    }
}

[System.Serializable]
public class CreatureConfigClass : Database<CreatureConfigAsset> { };

[System.Serializable]
public class CreatureConfigAsset : DatabaseAsset<CreatureConfig>
{
    public CreatureConfigAsset(string name, short databaseId16, short assetId, CreatureConfig objectReference)
        : base(name, databaseId16, assetId, objectReference) { }
}