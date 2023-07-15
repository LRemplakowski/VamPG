using UnityEngine;

[System.Serializable, CreateAssetMenu(menuName = "Custom/Item Data/Item Base Object")]
public class ItemBaseSObject : ScriptableObject
{
    [TextArea(4, 10)]
    public string description = "A short description";

    public int maxStackSize = 10;

    public float weight = 1.0f;

    public ItemCategorySObject category;

    public ItemRaritySObject rarity;

}