using UnityEditor;
using UnityEngine;

namespace SunsetSystems.Inventory.Inspector
{
    [CustomEditor(typeof(InventoryConfig))]
    public class ConfigEditor : Editor
    {
        private InventoryConfig _inventoryConfig;
        private SerializedObject _serializedObject;
        private SerializedProperty _serializedProperty;
        private Vector2 _scrollPosition;
        private GUIStyle _attributeListStyle;
        [SerializeField]
        private Texture2D _backgroundTexture;

        private void OnEnable()
        {
            _inventoryConfig = target as InventoryConfig;
            _serializedObject = new SerializedObject(_inventoryConfig);
            _serializedProperty = _serializedObject.FindProperty("_attributeList");
            _attributeListStyle = new GUIStyle();
            _attributeListStyle.normal.background = _backgroundTexture;
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();
            if (GUILayout.Button("Generate Inventory Data"))
            {
                _inventoryConfig.GenerateInventoryData();
            }
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            GUILayout.Label("Item Attributes", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("New Int"))
            {
                _inventoryConfig.AddAttribute(Data.ValueType.Integer);
            }
            if (GUILayout.Button("New Float"))
            {
                _inventoryConfig.AddAttribute(Data.ValueType.Float);
            }
            if (GUILayout.Button("New Bool"))
            {
                _inventoryConfig.AddAttribute(Data.ValueType.Bool);
            }
            if (GUILayout.Button("New String"))
            {
                _inventoryConfig.AddAttribute(Data.ValueType.String);
            }
            if (GUILayout.Button("New Script"))
            {
                _inventoryConfig.AddAttribute(Data.ValueType.Script);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(_attributeListStyle);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true), GUILayout.MaxHeight(250f), GUILayout.MinHeight(10f));
            for (int i = 0; i < _serializedProperty.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                SerializedProperty _attribute = _serializedProperty.GetArrayElementAtIndex(i);
                SerializedProperty _name = _attribute.FindPropertyRelative("_name");
                GUILayout.Label(_name.stringValue.ToUpper(), EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_name);
                SerializedProperty _valueType = _attribute.FindPropertyRelative("_valueType");
                EditorGUILayout.PropertyField(_valueType);
                GUILayout.EndVertical();
                if (GUILayout.Button("x"))
                {
                    _inventoryConfig.RemoveAttributeAtIndex(i);
                }
                EditorGUILayout.EndHorizontal();
                DrawUILine(Color.white);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            _serializedObject.ApplyModifiedProperties();
        }

        private void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
    }
}
