using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    public class ItemAttribute : ScriptableObject
    {
        [SerializeField, ReadOnly]
        internal string _fieldName;
        [SerializeField, ReadOnly]
        internal string _fieldType;
        [SerializeField, HideInInspector]
        internal List<string> _fieldAttributes = new();
    }
}
