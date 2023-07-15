using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Item Data/Item Category Object")]
public class ItemCategorySObject : ScriptableObject
{
    [Multiline(4)]
    public string description;
}
