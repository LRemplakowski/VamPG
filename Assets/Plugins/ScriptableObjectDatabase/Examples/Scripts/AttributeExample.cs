using MyLib.Shared.Database;
using UnityEngine;

[System.Serializable]
public class AttributeExample : MonoBehaviour
{
    public TextMesh text;
    [DatabaseAssetProperty(typeof(ItemDatabaseFile))]
    public int[] assetList;

    [DatabaseAssetProperty(typeof(PrefabDatabaseFile))]
    public int PrefabAssets;

    [DatabaseAssetProperty(typeof(ItemDatabaseFile))]
    public int ItemAsset0;

    [DatabaseAssetProperty(typeof(ItemDatabaseFile))]
    public int ItemAsset1;

    public void Start()
    {
        text.text = "Asset 0:" + MainDatabaseCollection.GetAsset(assetList[0]).name;
        Debug.Log("Asset 0:" + MainDatabaseCollection.GetAsset(assetList[0]).name);
        Debug.Log("Asset 1:" + MainDatabaseCollection.GetAsset(PrefabAssets).name);
        Debug.Log("Asset 2:" + MainDatabaseCollection.GetAsset(ItemAsset0).name);
        Debug.Log("Asset 3:" + MainDatabaseCollection.GetAsset(ItemAsset1).name);
    }
}
