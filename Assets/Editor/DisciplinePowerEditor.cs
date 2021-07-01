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

        SerializedProperty target = serializedObject.FindProperty("_target");
        EditorGUILayout.PropertyField(target);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("range"));
        SerializedProperty hasDiciplinePool = serializedObject.FindProperty("hasDiciplinePool");
        EditorGUILayout.PropertyField(hasDiciplinePool);
        bool hasDisciplinePoolValue = hasDiciplinePool.boolValue;
        if (hasDisciplinePoolValue)
        {
            EditorGUI.indentLevel++;
            SerializedProperty disciplinePool = serializedObject.FindProperty("disciplinePool");
            DisplayRollField(disciplinePool, true);
            DisplayRollField(disciplinePool, false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("disciplineRollDifficulty"));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
        SerializedProperty hasAttackPool = serializedObject.FindProperty("hasAttackPool");
        EditorGUILayout.PropertyField(hasAttackPool);
        bool hasAttackPoolValue = hasAttackPool.boolValue;
        if (hasAttackPoolValue)
        {
            EditorGUI.indentLevel++;
            SerializedProperty attackPool = serializedObject.FindProperty("attackPool");
            DisplayRollField(attackPool, true);
            DisplayRollField(attackPool, false);
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
        }
        SerializedProperty hasDefenseField = serializedObject.FindProperty("hasDefensePool");
        EditorGUILayout.PropertyField(hasDefenseField);
        bool hasDefenseRoll = hasDefenseField.boolValue;
        if (hasDefenseRoll)
        {
            EditorGUI.indentLevel++;
            SerializedProperty defensePool = serializedObject.FindProperty("defensePool");
            DisplayRollField(defensePool, true);
            DisplayRollField(defensePool, false);
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetableCreatureType"));

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
                    DisplayAttribute(effect, "attributeEffect");
                    break;
                case EffectType.Skill:
                    DisplayAttribute(effect, "skillEffect");
                    break;
                case EffectType.Discipline:
                    DisplayAttribute(effect, "disciplineEffect");
                    break;
                case EffectType.Tracker:
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

    private void DisplayTracker(SerializedProperty effect)
    {
        SerializedProperty effectProperty = effect.FindPropertyRelative("trackerEffect");
        EditorGUILayout.PropertyField(effectProperty.FindPropertyRelative("affectedValue"));
        DisplayCommonPropertyFields(effect, effectProperty);
    }

    private void DisplayAttribute(SerializedProperty effect, string effectPropertyName)
    {
        SerializedProperty effectProperty = effect.FindPropertyRelative(effectPropertyName);
        DisplayCommonPropertyFields(effect, effectProperty);
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

    private void DisplayCommonPropertyFields(SerializedProperty effect, SerializedProperty effectProperty)
    {
        SerializedProperty affectedCreature = effect.FindPropertyRelative("_affectedCreature");
        EditorGUILayout.PropertyField(affectedCreature);
        SerializedProperty duration = effect.FindPropertyRelative("duration");
        EditorGUILayout.PropertyField(duration);
        Duration d = (Duration)duration.enumValueIndex;
        if (d.Equals(Duration.Rounds))
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(effect.FindPropertyRelative("rounds"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(effectProperty.FindPropertyRelative("affectedProperty"));
        EditorGUILayout.PropertyField(effectProperty.FindPropertyRelative("modifierType"));
        SerializedProperty attributeModifierField = effectProperty.FindPropertyRelative("propertyModifier");
        EditorGUILayout.PropertyField(attributeModifierField);
        EffectModifier attributeModifier = (EffectModifier)attributeModifierField.enumValueIndex;
        EditorGUI.indentLevel++;
        switch (attributeModifier)
        {
            case EffectModifier.LevelBased:
                break;
            case EffectModifier.RollBased:
                break;
            case EffectModifier.StaticValue:
                EditorGUILayout.PropertyField(effectProperty.FindPropertyRelative("modifierValue"));
                break;
        }
        EditorGUI.indentLevel--;
    }
}
