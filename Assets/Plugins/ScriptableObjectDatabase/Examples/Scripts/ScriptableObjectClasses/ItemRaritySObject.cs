using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Item Data/Item Rarity Object")]
public class ItemRaritySObject : ScriptableObject
{
    [Multiline(4)]
    public string description;

    public Color color;

    [Range(0, 100)]
    public int dropChance = 25;
}
