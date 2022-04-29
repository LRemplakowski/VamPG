using System;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [Serializable]
    public class ItemAttributeTemplate
    {
        [SerializeField]
        private string _name = "Item Attribute";
        public string Name => _name;

        [SerializeField]
        private ValueType _valueType;
        public ValueType ValueType => _valueType;

        public ItemAttributeTemplate(ValueType _valueType)
        {
            this._valueType = _valueType;
        }
    }

    public enum ValueType
    {
        Integer, Float, String, Bool, Script
    }
}
