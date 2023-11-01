using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Core
{
    public interface IMaterialConfig
    {
        public IEnumerable<MaterialPropertyData> PropertyOverrides { get; }
    }

    [CreateAssetMenu(fileName = "New Material Config", menuName = "Sunset Core/Material Properties Config")]
    public class ScriptableMaterialConfig : SerializedScriptableObject, IMaterialConfig
    {
        [SerializeField, OnValueChanged("OnMaterialChanged")]
        private Material material;
        [SerializeField, HideInInspector]
        private Dictionary<string, MaterialPropertyType> materialParamData = new();
        [SerializeField, ValueDropdown("GetParamNamesExcludingExisting"), InlineButton("AddOverride", SdfIconType.Plus, "Add")]
        private string addPropertyOverride = "Property Name";
        [Title("Existing Overrides")]
        [SerializeField, Searchable, HideIf("@this.propertyOverrides != null")]
        private HashSet<MaterialPropertyData> propertyOverrides = new();
        [SerializeField, Searchable]
        private List<MaterialPropertyData> propertyOverrideList;
        public IEnumerable<MaterialPropertyData> PropertyOverrides => propertyOverrideList.AsEnumerable();

        #region Editor
        private void OnMaterialChanged()
        {
            addPropertyOverride = "Property Name";
            propertyOverrideList = new();
        }

        private void AddOverride()
        {
            if (addPropertyOverride == "Property Name")
            {
                Debug.LogError("Select property to override!");
                return;
            }
            MaterialPropertyData data = new(addPropertyOverride, materialParamData[addPropertyOverride]);
            if (propertyOverrideList.Any(d => d.PropertyName == data.PropertyName))
            {
                Debug.LogError($"Material Property Override for property {addPropertyOverride} already exists!");
            }
            else
            {
                propertyOverrideList.Add(data);
                addPropertyOverride = "Property Name";
            }
        }

        private List<string> GetParamNamesExcludingExisting()
        {
            return materialParamData.Keys.Except(propertyOverrideList.Select(p => p.PropertyName)).ToList();
        }

        private void OnValidate()
        {
            if (propertyOverrideList == null || propertyOverrideList.Count == 0)
                propertyOverrideList = propertyOverrides.ToList();
            materialParamData = new();
            propertyOverrideList ??= new();
            foreach (MaterialPropertyType propertyType in Enum.GetValues(typeof(MaterialPropertyType)))
            {
                if (propertyType == MaterialPropertyType.ConstantBuffer || propertyType == MaterialPropertyType.ComputeBuffer)
                    continue;
                string[] propNames = material.GetPropertyNames(propertyType);
                CachePropertyNames(propertyType, propNames);
            }

            void CachePropertyNames(MaterialPropertyType propertyType, string[] propNames)
            {
                foreach (string propName in propNames)
                {
                    if (string.IsNullOrWhiteSpace(propName))
                        continue;
                    materialParamData[propName] = propertyType;
                }
            }
        }
    }
    #endregion

    [Serializable]
    public struct MaterialPropertyData
    {
        [field: SerializeField, ReadOnly]
        public string PropertyName { get; private set; }
        [field: SerializeField, ReadOnly]
        public MaterialPropertyType PropertyType { get; private set; }
        [SerializeField, ShowIf("@PropertyType == UnityEngine.MaterialPropertyType.Float")]
        private float floatValue;
        [SerializeField, ShowIf("@PropertyType == UnityEngine.MaterialPropertyType.Int")]
        private int intValue;
        [SerializeField, ShowIf("@PropertyType == UnityEngine.MaterialPropertyType.Vector")]
        private Vector4 vectorValue;
        [SerializeField, ShowIf("@PropertyType == UnityEngine.MaterialPropertyType.Matrix")]
        private Matrix4x4 matrixValue;
        [SerializeField, ShowIf("@PropertyType == UnityEngine.MaterialPropertyType.Texture")]
        private Texture textureValue;

        public MaterialPropertyData(string PropertyName, MaterialPropertyType PropertyType)
        {
            this.PropertyName = PropertyName;
            this.PropertyType = PropertyType;
            floatValue = default;
            intValue = default;
            vectorValue = default;
            matrixValue = default;
            textureValue = default;
        }

        public object GetValue()
        {
            return PropertyType switch
            {
                MaterialPropertyType.Float => floatValue,
                MaterialPropertyType.Int => intValue,
                MaterialPropertyType.Vector => vectorValue,
                MaterialPropertyType.Matrix => matrixValue,
                MaterialPropertyType.Texture => textureValue,
                _ => null,
            };
        }

        public T GetValue<T>()
        {
            return (T) GetValue();
        }
    }
}
