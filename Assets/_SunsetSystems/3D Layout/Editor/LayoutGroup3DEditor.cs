#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LayoutGroup3D))]
public class LayoutGroup3DEditor : Editor
{
    private LayoutGroup3D LayoutGroup;

    private LayoutStyle Style;
    private float Spacing;
    private Vector3 ElementDimensions;

    private int GridConstraintCount;
    private int SecondaryConstraintCount;

    private bool UseFullCircle;
    private float MaxArcAngle;
    private float Radius;
    private float StartAngleOffset;
    private bool AlignToRadius;
    private LayoutAxis3D PrimaryAlignmentAxis;
    private LayoutAxis3D SecondaryAlignmentAxis;
    private float SpiralFactor;
    private LayoutAxis3D LayoutAxis;
    private LayoutAxis3D SecondaryLayoutAxis;
    private Vector3 StartPositionOffset;
    private Alignment PrimaryAlignment;
    private Alignment SecondaryAlignment;
    private Alignment TertiaryAlignment;

    public override void OnInspectorGUI()
    {

        LayoutGroup = target as LayoutGroup3D;

        DrawDefaultInspector();

        bool shouldRebuild = false;

        // Record rotations of all children if not forcing alignment in radial mode
        if (!(LayoutGroup.Style == LayoutStyle.Radial && LayoutGroup.AlignToRadius))
        {
            LayoutGroup.RecordRotations();
        }

        // Element Dimensions
        EditorGUI.BeginChangeCheck();

        ElementDimensions = EditorGUILayout.Vector3Field(new GUIContent("Element Dimensions", ToolTip_ElemDimensions), LayoutGroup.ElementDimensions);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(LayoutGroup, "Change Element Dimensions");
            LayoutGroup.ElementDimensions = ElementDimensions;
            shouldRebuild = true;
        }

        // Start Offset
        EditorGUI.BeginChangeCheck();

        StartPositionOffset = EditorGUILayout.Vector3Field("Start Position Offset", LayoutGroup.StartPositionOffset);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(LayoutGroup, "Change Position Offset");
            LayoutGroup.StartPositionOffset = StartPositionOffset;
            shouldRebuild = true;
        }

        EditorGUI.BeginChangeCheck();

        Style = (LayoutStyle)EditorGUILayout.EnumPopup("Layout Style", LayoutGroup.Style);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(LayoutGroup, "Change Layout Style");
            LayoutGroup.Style = Style;
            shouldRebuild = true;
        }

        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();

        if (Style == LayoutStyle.Linear)
        {
            LayoutAxis = (LayoutAxis3D)EditorGUILayout.EnumPopup("Layout Axis", LayoutGroup.LayoutAxis);
            PrimaryAlignment = (Alignment)EditorGUILayout.EnumPopup("Alignment", LayoutGroup.PrimaryAlignment);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(LayoutGroup, "Change Layout Axis");
                LayoutGroup.LayoutAxis = LayoutAxis;
                LayoutGroup.PrimaryAlignment = PrimaryAlignment;
                shouldRebuild = true;
            }
        }
        else if (Style == LayoutStyle.Grid)
        {
            LayoutAxis = (LayoutAxis3D)EditorGUILayout.EnumPopup("Primary Layout Axis", LayoutGroup.LayoutAxis);
            SecondaryLayoutAxis = (LayoutAxis3D)EditorGUILayout.EnumPopup("Secondary Layout Axis", LayoutGroup.SecondaryLayoutAxis);
            GridConstraintCount = EditorGUILayout.IntField("Constraint Count", LayoutGroup.GridConstraintCount);

            string pAlignStr = "Primary Alignment";
            string sAlignStr = "Secondary Alignment";

            PrimaryAlignment = (Alignment)EditorGUILayout.EnumPopup(pAlignStr, LayoutGroup.PrimaryAlignment);
            SecondaryAlignment = (Alignment)EditorGUILayout.EnumPopup(sAlignStr, LayoutGroup.SecondaryAlignment);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(LayoutGroup, "Change Grid Layout Options");
                LayoutGroup.GridConstraintCount = GridConstraintCount;
                LayoutGroup.LayoutAxis = LayoutAxis;
                LayoutGroup.SecondaryLayoutAxis = SecondaryLayoutAxis;
                LayoutGroup.PrimaryAlignment = PrimaryAlignment;
                LayoutGroup.SecondaryAlignment = SecondaryAlignment;
                shouldRebuild = true;
            }
        }
        else if (Style == LayoutStyle.Euclidean)
        {
            LayoutAxis = (LayoutAxis3D)EditorGUILayout.EnumPopup("Primary Layout Axis", LayoutGroup.LayoutAxis);
            SecondaryLayoutAxis = (LayoutAxis3D)EditorGUILayout.EnumPopup("Secondary Layout Axis", LayoutGroup.SecondaryLayoutAxis);

            GridConstraintCount = EditorGUILayout.IntField("Primary Constraint Count", LayoutGroup.GridConstraintCount);
            SecondaryConstraintCount = EditorGUILayout.IntField("Secondary Constraint Count", LayoutGroup.SecondaryConstraintCount);

            PrimaryAlignment = (Alignment)EditorGUILayout.EnumPopup("Primary Alignment", LayoutGroup.PrimaryAlignment);
            SecondaryAlignment = (Alignment)EditorGUILayout.EnumPopup("Secondary Alignment", LayoutGroup.SecondaryAlignment);
            TertiaryAlignment = (Alignment)EditorGUILayout.EnumPopup("Tertiary Alignment", LayoutGroup.TertiaryAlignment);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(LayoutGroup, "Change Euclidean Layout Options");
                LayoutGroup.GridConstraintCount = GridConstraintCount;
                LayoutGroup.SecondaryConstraintCount = SecondaryConstraintCount;
                LayoutGroup.LayoutAxis = LayoutAxis;
                LayoutGroup.SecondaryLayoutAxis = SecondaryLayoutAxis;
                LayoutGroup.PrimaryAlignment = PrimaryAlignment;
                LayoutGroup.SecondaryAlignment = SecondaryAlignment;
                LayoutGroup.TertiaryAlignment = TertiaryAlignment;
                shouldRebuild = true;
            }
        }
        else if (Style == LayoutStyle.Radial)
        {
            LayoutAxis = (LayoutAxis3D)EditorGUILayout.EnumPopup(new GUIContent("Plane Normal Axis", ToolTip_Radial_LayoutAxis), LayoutGroup.LayoutAxis);
            UseFullCircle = EditorGUILayout.Toggle(new GUIContent("Use Full Circle", ToolTip_Radial_UseFullCircle), LayoutGroup.UseFullCircle);
            if(!UseFullCircle)
            {
                MaxArcAngle = EditorGUILayout.FloatField(new GUIContent("Max Arc Angle", ToolTip_Radial_MaxArcAngle), LayoutGroup.MaxArcAngle);
            }
            else
            {
                int childCount = LayoutGroup.transform.childCount;
                MaxArcAngle = 360f - 360f / childCount;
            }
            Radius = EditorGUILayout.FloatField("Radius", LayoutGroup.Radius);
            StartAngleOffset = EditorGUILayout.FloatField("Start Angle Offset", LayoutGroup.StartAngleOffset);
            SpiralFactor = EditorGUILayout.FloatField("Spiral Factor", LayoutGroup.SpiralFactor);
            AlignToRadius = EditorGUILayout.Toggle(new GUIContent("Align To Radius", ToolTip_Radial_AlignToRadius), LayoutGroup.AlignToRadius);

            if(AlignToRadius)
            {
                PrimaryAlignmentAxis = (LayoutAxis3D)EditorGUILayout.EnumPopup(new GUIContent("Primary Alignment Axis", ToolTip_Radial_PrimaryAlign), LayoutGroup.PrimaryAlignmentAxis);
                SecondaryAlignmentAxis = (LayoutAxis3D)EditorGUILayout.EnumPopup(new GUIContent("Secondary Alignment Axis", ToolTip_Radial_SecondaryAlign), LayoutGroup.SecondaryAlignmentAxis);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(LayoutGroup, "Change Radial Layout Options");
                LayoutGroup.LayoutAxis = LayoutAxis;
                LayoutGroup.UseFullCircle = UseFullCircle;
                LayoutGroup.MaxArcAngle = MaxArcAngle;
                LayoutGroup.Radius = Radius;
                LayoutGroup.StartAngleOffset = StartAngleOffset;
                LayoutGroup.SpiralFactor = SpiralFactor;
                LayoutGroup.AlignToRadius = AlignToRadius;
                LayoutGroup.PrimaryAlignmentAxis = PrimaryAlignmentAxis;
                LayoutGroup.SecondaryAlignmentAxis = SecondaryAlignmentAxis;
                shouldRebuild = true;
            }
        }

        if (LayoutGroup.Style != LayoutStyle.Radial)
        {
            EditorGUI.BeginChangeCheck();
            Spacing = EditorGUILayout.FloatField(new GUIContent("Spacing", ToolTip_Spacing), LayoutGroup.Spacing);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(LayoutGroup, "Change spacing");
                LayoutGroup.Spacing = Spacing;
                shouldRebuild = true;
            }
        }

        if (!(LayoutGroup.Style == LayoutStyle.Radial && LayoutGroup.AlignToRadius))
        {
            LayoutGroup.RestoreRotations();
        }

        if (shouldRebuild || LayoutGroup.NeedsRebuild || EditorUtility.IsDirty(LayoutGroup.transform))
        {
            LayoutGroup.RebuildLayout();
        }


    }

    private void OnEnable()
    {
        Undo.undoRedoPerformed += ForceRebuild;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= ForceRebuild;
    }

    void ForceRebuild()
    {
        if(LayoutGroup)
        {
            LayoutGroup.RebuildLayout();
        }
    }

    private const string ToolTip_ElemDimensions = "The size of child elements in local space used to determine the distance between their pivot points in the layout.  Not used in Radial Layout.";
    private const string ToolTip_Spacing = "Additional spacing applied between elements";
    private const string ToolTip_Radial_LayoutAxis = "The direction of the plane normal containing the circle";
    private const string ToolTip_Radial_UseFullCircle = "If true, angle between elements is automatically calculated so that they evenly fill a circle";
    private const string ToolTip_Radial_MaxArcAngle = "Elements will be distributed around the circle between 0 and this angle";
    private const string ToolTip_Radial_AlignToRadius = "If true, Elements will be rotated to align one axis pointing outward from the circle center, and another axis aligned with the plane normal";
    private const string ToolTip_Radial_PrimaryAlign = "The transform axis of the elements that should point outward from the center of the circle.";
    private const string ToolTip_Radial_SecondaryAlign = "The transform axis of the elements that should point in the same direction as the plane normal.";
}

#endif
