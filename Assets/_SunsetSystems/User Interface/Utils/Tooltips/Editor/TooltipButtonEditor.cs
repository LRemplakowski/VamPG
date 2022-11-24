using UnityEditor;
using UnityEditor.UI;

namespace SunsetSystems.UI.Editor
{
    [CustomEditor(typeof(TooltipButton))]
    public class TooltipButtonEditor : ButtonEditor
    {
        SerializedProperty _tooltipProperty, _tooltipParentProperty, _tooltipDelayProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _tooltipProperty = serializedObject.FindProperty("_tooltip");
            _tooltipParentProperty = serializedObject.FindProperty("_tooltipParent");
            _tooltipDelayProperty = serializedObject.FindProperty("_tooltipDelay");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_tooltipProperty);
            EditorGUILayout.PropertyField(_tooltipParentProperty);
            EditorGUILayout.PropertyField(_tooltipDelayProperty);
            if (serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
