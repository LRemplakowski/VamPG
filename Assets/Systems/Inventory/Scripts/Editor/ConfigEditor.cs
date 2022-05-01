using UnityEditor;
using UnityEngine;

namespace SunsetSystems.Inventory.Inspector
{
    [CustomEditor(typeof(InventoryConfig))]
    public class ConfigEditor : Editor
    {
        private InventoryConfig _inventoryConfig;
        private SerializedObject _serializedObject;
        [SerializeField]
        private Texture2D _backgroundTexture;

        private void OnEnable()
        {
            _inventoryConfig = target as InventoryConfig;
            _serializedObject = new SerializedObject(_inventoryConfig);
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();
            if (GUILayout.Button("Generate Inventory Data"))
            {
                _ = _inventoryConfig.GenerateInventoryDataAsync();
            }
            if (GUILayout.Button("Generate Item Types"))
                _ = _inventoryConfig.GenerateItemTypesAsync();
            base.OnInspectorGUI();
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
