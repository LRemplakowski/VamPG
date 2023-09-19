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
        [SerializeField, Searchable]
        private HashSet<MaterialPropertyData> propertyOverrides = new();
        public IEnumerable<MaterialPropertyData> PropertyOverrides => propertyOverrides.AsEnumerable();

        #region Editor
#if UNITY_EDITOR
#pragma warning disable IDE0051 // Remove unused private members
        private void OnMaterialChanged()
        {
            addPropertyOverride = "Property Name";
            propertyOverrides = new();
        }

        private void AddOverride()
        {
            if (addPropertyOverride == "Property Name")
            {
                Debug.LogError("Select property to override!");
                return;
            }
            MaterialPropertyData data = new(addPropertyOverride, materialParamData[addPropertyOverride]);
            bool success = propertyOverrides.Add(data);
            if (success)
                addPropertyOverride = "Property Name";
            else
                Debug.LogError($"Material Property Override for property {addPropertyOverride} already exists!");
        }

        private List<string> GetParamNamesExcludingExisting()
        {
            return materialParamData.Keys.Except(propertyOverrides.Select(p => p.PropertyName)).ToList();
        }
#pragma warning restore IDE0051 // Remove unused private members

        private void OnValidate()
        {
            materialParamData = new();
            propertyOverrides ??= new();
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
#endif
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
        private AssetReferenceTexture textureValue;

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
            return null;
        }

        public T GetValue<T>()
        {
            return (T) GetValue();
        }

        public override bool Equals(object obj)
        {
            return obj is MaterialPropertyData data && PropertyName == data.PropertyName;
        }

        public override int GetHashCode()
        {
            return PropertyName.GetHashCode();
        }
    }
}
