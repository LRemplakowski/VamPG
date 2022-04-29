using UnityEngine;
using SunsetSystems.Utils.Generation;
using System.Collections.Generic;
using System.Linq;
using SunsetSystems.Inventory.Data;
using UnityEditor;

namespace SunsetSystems.Inventory
{
    [CreateAssetMenu(fileName = "Inventory Config", menuName = "Inventory/Config")]
    public class InventoryConfig : ScriptableObject
    {
        [Header("Item Categories")]
        [SerializeField]
        private string _categoryDataPath;
        [SerializeField]
        private string _categoryEnumName;
        [SerializeField]
        private string[] _categoryList = new string[0];
        [Header("Equipment Slots")]
        [SerializeField]
        private EquipmentSlot[] _slotCategories = new EquipmentSlot[0];
        public EquipmentSlot[] SlotCategories => _slotCategories.Distinct().ToArray();
        [Header("Item Attributes")]
        [SerializeField]
        private string _itemAttributeDataPath;
        [SerializeField, HideInInspector]
        private List<ItemAttributeTemplate> _attributeList = new();

        private void OnEnable()
        {
            _categoryDataPath = "/Systems/Inventory/Scripts/";
            _categoryEnumName = "ItemCategory";
            _itemAttributeDataPath = "/Systems/Inventory/Config/Item Attributes/";
        }

        public void AddAttribute(ValueType valueType)
        {
            ItemAttributeTemplate attribute = new(valueType);
            _attributeList.Add(attribute);
        }

        public void RemoveAttributeAtIndex(int index)
        {
            _attributeList.RemoveAt(index);
        }

        public async void GenerateInventoryData()
        {
            List<string> categories = _categoryList.Distinct().ToList();
            await CodeGenerator.GenerateEnumAsync(categories, Application.dataPath + _categoryDataPath, _categoryEnumName, "SunsetSystems.Inventory.Data");
            AssetDatabase.ImportAsset("Assets" + _categoryDataPath + _categoryEnumName + ".cs");
            foreach (ItemAttributeTemplate template in _attributeList)
            {
                CreateItemAttributeAsset(template.Name, System.Type.GetType(template.ValueType.ToString()), "Assets" + _itemAttributeDataPath);
            }
        }

        private void CreateItemAttributeAsset(string assetName, System.Type valueType, string dataPath)
        {
            ItemAttribute attribute = CreateInstance<ItemAttribute>();
            attribute.AttributeName = assetName;
            attribute.ValueType = valueType;
            AssetDatabase.CreateAsset(attribute, dataPath + assetName + ".asset");
            AssetDatabase.SaveAssets();
        }
    }
}
