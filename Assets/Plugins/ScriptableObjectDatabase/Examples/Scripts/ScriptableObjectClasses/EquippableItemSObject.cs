using UnityEngine;

[System.Serializable, CreateAssetMenu(menuName = "Custom/Item Data/Equippable Item Object")]
public class EquippableItemSObject : ItemBaseSObject
{
    public EquippableItemTypeSObject equipmentType;
    public float level = 1;
}