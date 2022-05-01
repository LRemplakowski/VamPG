using UnityEngine;
using SunsetSystems.Utils.Generation;
using System.Collections.Generic;
using System.Linq;
using SunsetSystems.Inventory.Data;
using UnityEditor;
using System;
using SunsetSystems.Utils;
using System.Threading.Tasks;

namespace SunsetSystems.Inventory
{
    [CreateAssetMenu(fileName = "Inventory Config", menuName = "Inventory/Config")]
    public class InventoryConfig : ScriptableObject
    {
        [SerializeField]
        private SortingCategories _sortingCategories;
        private string CategoryDataPath { get => _sortingCategories.dataPath; set => _sortingCategories.dataPath = value; }
        private string CategoryEnumName { get => _sortingCategories.enumName; set => _sortingCategories.enumName = value; }
        private string[] Categories { get => _sortingCategories.categoryList; set => _sortingCategories.categoryList = value; }
        [SerializeField]
        private EquipmentSlots _equipmentSlots;
        public IReadOnlyList<EquipmentSlot> SlotCategories => _equipmentSlots.slotCategories.Distinct().ToList();
        [SerializeField]
        private ItemAttributes _itemAttributes;
        private string ItemAttributeDataPath { get => _itemAttributes.dataPath; set => _itemAttributes.dataPath = value; }
        private ItemAttributeTemplate[] ItemAttributeTemplates { get => _itemAttributes.templates; set => _itemAttributes.templates = value; }
        [SerializeField]
        private ItemTypes _itemTypes;
        private string ItemTypeDataPath { get => _itemTypes.dataPath; set => _itemTypes.dataPath = value; }
        private string ItemTypeEnumName { get => _itemTypes.enumName; set => _itemTypes.enumName = value; }
        private ItemTypeTemplate[] ItemTypeTemplates => _itemTypes.typeTemplates;

        private readonly List<string> _itemAttributePaths = new();
        private readonly List<string> _itemTypePaths = new();
        private readonly List<string> _itemTypeNames = new();


        private void OnEnable()
        {
            CategoryDataPath = "/Systems/Inventory/Scripts/";
            CategoryEnumName = "ItemCategory";
            ItemAttributeDataPath = "/Systems/Inventory/Config/Item Attributes/";
            ItemTypeDataPath = "/Systems/Inventory/Scripts/";
            ItemTypeEnumName = "ItemType";
        }

        private void OnValidate()
        {
            for (int i = 0; i < Categories.Length; i++)
            {
                string category = Categories[i];
                if (category.Length == 0)
                    category = "Item Category";
                Categories[i] = category;
            }
            for (int i = 0; i < ItemAttributeTemplates.Length; i++)
            {
                ItemAttributeTemplate template = ItemAttributeTemplates[i];
                if (template.name.Length == 0)
                    template.name = "Item Attribute";
            }
            foreach (ItemTypeTemplate template in ItemTypeTemplates)
            {
                if (template.name.Length == 0)
                    template.name = "Item Type";
            }
        }

        public async Task GenerateInventoryDataAsync()
        {
            List<string> categories = Categories.Distinct().ToList();
            await CodeGenerator.GenerateEnumAsync(categories, Application.dataPath + CategoryDataPath, CategoryEnumName, "SunsetSystems.Inventory.Data");
            AssetDatabase.ImportAsset("Assets" + CategoryDataPath + CategoryEnumName + ".cs");
            ClearItemAttributes();
            foreach (ItemAttributeTemplate template in ItemAttributeTemplates)
            {
                CreateItemAttributeAsset(template.name, template.valueType, "Assets" + ItemAttributeDataPath);
                await Task.Yield();
            }
            AssetDatabase.SaveAssets();
        }

        public async Task GenerateItemTypesAsync()
        {
            _itemTypeNames.Clear();
            ClearItemTypes();
            foreach (ItemTypeTemplate template in ItemTypeTemplates)
            {
                _itemTypeNames.Add(template.name.RemoveSpecialCharacters());
                await GenerateItemTypeClassAsync(template);
            }
            CodeGenerator.GenerateEnum(_itemTypeNames, Application.dataPath + ItemTypeDataPath, ItemTypeEnumName, "SunsetSystems.Inventory.Data");
            AssetDatabase.ImportAsset("Assets" + ItemTypeDataPath + ItemTypeEnumName + ".cs");
            AssetDatabase.SaveAssets();
        }

        private string GetTypeStringFromValueTypeEnum(ValueType valueType)
        {
            return valueType switch
            {
                ValueType.Integer => "int",
                ValueType.Float => "float",
                ValueType.String => "string",
                ValueType.Bool => "bool",
                ValueType.Script => "ScriptableObject[]",
                _ => throw new ArgumentException("Value type " + valueType + " is not recognized!"),
            };
        }

        private async Task GenerateItemTypeClassAsync(ItemTypeTemplate template)
        {
            string dataPath = Application.dataPath + ItemTypeDataPath;
            string className = template.name.RemoveSpecialCharacters();
            _itemTypePaths.Add("Assets" + ItemTypeDataPath + className + ".cs");
            string nameSpace = "SunsetSystems.Inventory.Data";
            string createAssetMenuAttribute = "[CreateAssetMenu(fileName = \"New " + template.name + "\", menuName = \"Inventory/Items/" + template.name + "\")]";
            string baseClass = nameof(BaseItem);
            List<string> usingDirectives = new() { "UnityEngine", "SunsetSystems.Inventory", "System.Collections.Generic" };
            CodeGenerator.ClassBuilder classBuilder = new(className, dataPath);
            classBuilder.SetNameSpace(nameSpace)
                .SetBaseClass(baseClass)
                .SetClassAttributes(createAssetMenuAttribute);
            foreach (string usingDirective in usingDirectives)
            {
                classBuilder.AddUsingDirective(usingDirective);
                await Task.Yield();
            }
            foreach (ItemAttribute itemAttribute in template.itemTypeAttributes)
            {
                List<string> fieldAttributes = new() { "SerializeField" };
                fieldAttributes.AddRange(itemAttribute._fieldAttributes);
                classBuilder.AddPrivateField(itemAttribute._fieldName, itemAttribute._fieldType, fieldAttributes.ToArray());
                await Task.Yield();
            }
            await CodeGenerator.GenerateScriptAsync(classBuilder.Build());
            AssetDatabase.ImportAsset("Assets" + ItemTypeDataPath + className + ".cs");
        }

        private string GetAttributeFromValueType(ValueType valueType)
        {
            return valueType switch
            {
                ValueType.Integer => "",
                ValueType.Float => "",
                ValueType.String => "",
                ValueType.Bool => "",
                ValueType.Script => "RequireInterface(typeof(IScriptableItemAttribute))",
                _ => "",
            };
        }

        private void ClearItemAttributes()
        {
            AssetDatabase.DeleteAssets(_itemAttributePaths.ToArray(), new List<string>());
            _itemAttributePaths.Clear();
        }

        private void ClearItemTypes()
        {
            AssetDatabase.DeleteAssets(_itemTypePaths.ToArray(), new List<string>());
            _itemTypePaths.Clear();
        }

        private void CreateItemAttributeAsset(string assetName, ValueType valueType, string dataPath)
        {
            ItemAttribute attribute = CreateInstance<ItemAttribute>();
            attribute._fieldName = assetName;
            attribute._fieldType = GetTypeStringFromValueTypeEnum(valueType);
            attribute._fieldAttributes.Add(GetAttributeFromValueType(valueType));
            string assetPath = dataPath + assetName + ".asset";
            AssetDatabase.CreateAsset(attribute, assetPath);
            _itemAttributePaths.Add(assetPath);
        }

        [Serializable]
        private class SortingCategories
        {
            public string dataPath;
            public string enumName;
            public string[] categoryList = new string[0];
        }

        [Serializable]
        private class EquipmentSlots
        {
            public EquipmentSlot[] slotCategories = new EquipmentSlot[0];
        }

        [Serializable]
        private class ItemAttributes
        {
            public string dataPath;
            public ItemAttributeTemplate[] templates = new ItemAttributeTemplate[0];
        }

        [Serializable]
        private class ItemAttributeTemplate
        {
            public string name = "Item Attribute";
            public ValueType valueType;
        }

        [Serializable]
        private enum ValueType
        {
            Integer, Float, String, Bool, Script
        }

        [Serializable]
        private class ItemTypes
        {
            public string dataPath;
            public string enumName;
            public ItemTypeTemplate[] typeTemplates = new ItemTypeTemplate[0];
        }

        [Serializable]
        private class ItemTypeTemplate
        {
            public string name;
            public ItemAttribute[] itemTypeAttributes = new ItemAttribute[0];
        }
    }
}
