using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static DisciplinePower;

[CustomEditor(typeof(DisciplinePower))]
public class DisciplinePowerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("level")); 
        EditorGUILayout.PropertyField(serializedObject.FindProperty("secondaryType"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("secondaryLevel"));
        EditorGUILayout.Space();

        SerializedProperty effects = serializedObject.FindProperty("effects");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Number of effects:");
        int size = EditorGUILayout.IntField(effects.arraySize, GUILayout.Width(50));
        if (effects.arraySize != size)
        {
            effects.arraySize = size;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel++;
        for (int i = 0; i < effects.arraySize; i++)
        {
            DisplayEffect(effects.GetArrayElementAtIndex(i));
        }
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayEffect(SerializedProperty effect)
    {
        SerializedProperty effectTypeProperty = effect.FindPropertyRelative("effectType");
        EditorGUILayout.PropertyField(effectTypeProperty);
        EffectType effectCategory = (EffectType)effectTypeProperty.enumValueIndex;
        //effect.FindPropertyRelative("effectType").enumValueIndex = (int)(EffectType)EditorGUILayout.EnumPopup("Effect type", effectCategory);
        SerializedProperty expandedProperty = effect.FindPropertyRelative("isExpanded");
        bool isExpanded = expandedProperty.boolValue;
        EditorGUI.indentLevel++;
        isExpanded = EditorGUILayout.Foldout(isExpanded, "Properties", true);
        expandedProperty.boolValue = isExpanded;
        if (isExpanded)
        {
            EditorGUI.indentLevel++;
            switch (effectCategory)
            {
                case EffectType.Attribute:
                    EditorGUILayout.PropertyField(effect.FindPropertyRelative("target"));
                    DisplayAttribute(effect, "attributeEffect");
                    break;
                case EffectType.Skill:
                    EditorGUILayout.PropertyField(effect.FindPropertyRelative("target"));
                    DisplayAttribute(effect, "skillEffect");
                    break;
                case EffectType.Discipline:
                    EditorGUILayout.PropertyField(effect.FindPropertyRelative("target"));
                    DisplayAttribute(effect, "disciplineEffect");
                    break;
                case EffectType.Tracker:
                    EditorGUILayout.PropertyField(effect.FindPropertyRelative("target"));
                    DisplayTracker(effect);
                    break;
                case EffectType.ScriptDriven:
                    EditorGUILayout.PropertyField(effect.FindPropertyRelative("scriptEffect").FindPropertyRelative("script"));
                    break;
            }
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();
    }

    private void DisplayScriptEffect(SerializedProperty effect)
    {

    }

    private void DisplayTracker(SerializedProperty effect)
    {
        SerializedProperty effectProperty = effect.FindPropertyRelative("trackerEffect");
        EditorGUILayout.PropertyField(effectProperty.FindPropertyRelative("affectedProperty"));
        EditorGUILayout.PropertyField(effectProperty.FindPropertyRelative("affectedValue"));
        EditorGUILayout.PropertyField(effectProperty.FindPropertyRelative("modifierType"));
        SerializedProperty attributeModifierField = effectProperty.FindPropertyRelative("propertyModifier");
        EditorGUILayout.PropertyField(attributeModifierField);
        EffectModifier attributeModifier = (EffectModifier)attributeModifierField.enumValueIndex;
        EditorGUILayout.Space();
        EditorGUI.indentLevel++;
        switch (attributeModifier)
        {
            case EffectModifier.LevelBased:
                break;
            case EffectModifier.RollBased:
                SerializedProperty dicePool = effectProperty.FindPropertyRelative("disciplineRoll");
                DisplayRollField(dicePool, true);
                DisplayRollField(dicePool, false);
                break;
            case EffectModifier.StaticValue:
                EditorGUILayout.PropertyField(effectProperty.FindPropertyRelative("modifierValue"));
                break;
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();
        SerializedProperty hasDefenseField = effectProperty.FindPropertyRelative("hasDefenseRoll");
        EditorGUILayout.PropertyField(hasDefenseField);
        bool hasDefenseRoll = hasDefenseField.boolValue;
        if (hasDefenseRoll)
        {
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;
            SerializedProperty defensePool = effectProperty.FindPropertyRelative("defenseRoll");
            DisplayRollField(defensePool, true);
            DisplayRollField(defensePool, false);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space();
    }

    private void DisplayAttribute(SerializedProperty effect, string effectPropertyName)
    {
        SerializedProperty effectProperty = effect.FindPropertyRelative(effectPropertyName);
        EditorGUILayout.PropertyField(effectProperty.FindPropertyRelative("affectedProperty"));
        EditorGUILayout.PropertyField(effectProperty.FindPropertyRelative("modifierType"));
        SerializedProperty attributeModifierField = effectProperty.FindPropertyRelative("propertyModifier");
        EditorGUILayout.PropertyField(attributeModifierField);
        EffectModifier attributeModifier = (EffectModifier)attributeModifierField.enumValueIndex;
        EditorGUILayout.Space();
        EditorGUI.indentLevel++;
        switch (attributeModifier)
        {
            case EffectModifier.LevelBased:
                break;
            case EffectModifier.RollBased:
                SerializedProperty dicePool = effectProperty.FindPropertyRelative("disciplineRoll");
                DisplayRollField(dicePool, true);
                DisplayRollField(dicePool, false);
                break;
            case EffectModifier.StaticValue:
                EditorGUILayout.PropertyField(effectProperty.FindPropertyRelative("modifierValue"));
                break;
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();
        SerializedProperty hasDefenseField = effectProperty.FindPropertyRelative("hasDefenseRoll");
        EditorGUILayout.PropertyField(hasDefenseField);
        bool hasDefenseRoll = hasDefenseField.boolValue;
        if (hasDefenseRoll)
        {
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;
            SerializedProperty defensePool = effectProperty.FindPropertyRelative("defenseRoll");
            DisplayRollField(defensePool, true);
            DisplayRollField(defensePool, false);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space();
    }

    private void DisplayRollField(SerializedProperty dicePool, bool first)
    {
        SerializedProperty pool = dicePool.FindPropertyRelative(first ? "firstPool" : "secondPool");
        EditorGUILayout.PropertyField(pool);
        FieldType ft = (FieldType)pool.enumValueIndex;
        EditorGUI.indentLevel++;
        switch (ft)
        {
            case FieldType.Attribute:
                EditorGUILayout.PropertyField(dicePool.FindPropertyRelative(first ? "attribute" : "secondAttribute"));
                break;
            case FieldType.Skill:
                EditorGUILayout.PropertyField(dicePool.FindPropertyRelative(first ? "skill" : "secondSkill"));
                break;
            case FieldType.Discipline:
                EditorGUILayout.PropertyField(dicePool.FindPropertyRelative(first ? "discipline" : "secondDiscipline"));
                break;
        }
        EditorGUI.indentLevel--;
    }
}
