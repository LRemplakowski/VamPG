using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    public class ItemAttribute : ScriptableObject
    {
        [SerializeField]
        private string _attributeName;
        public string AttributeName { get => _attributeName; internal set => _attributeName = value; }
        [SerializeField]
        private System.Type _attributeValueType;
        public System.Type ValueType { get => _attributeValueType; internal set => _attributeValueType = value; }
    }
}
