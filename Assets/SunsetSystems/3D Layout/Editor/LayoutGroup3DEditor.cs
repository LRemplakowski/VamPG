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
    private float SpiralFactor;
    private LayoutAxis3D LayoutAxis;
    private LayoutAxis3D SecondaryLayoutAxis;
    private LayoutAxis2D GridLayoutAxis;
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

        ElementDimensions = EditorGUILayout.Vector3Field("Element Dimensions", LayoutGroup.ElementDimensions);

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
            GridLayoutAxis = (LayoutAxis2D)EditorGUILayout.EnumPopup("Primary Layout Axis", LayoutGroup.GridLayoutAxis);
            GridConstraintCount = EditorGUILayout.IntField("Constraint Count", LayoutGroup.GridConstraintCount);

            string pAlignStr = GridLayoutAxis == LayoutAxis2D.X ? "X Alignment" : "Y Alignment";
            string sAlignStr = GridLayoutAxis == LayoutAxis2D.X ? "Y Alignment" : "X Alignment";

            PrimaryAlignment = (Alignment)EditorGUILayout.EnumPopup(pAlignStr, LayoutGroup.PrimaryAlignment);
            SecondaryAlignment = (Alignment)EditorGUILayout.EnumPopup(sAlignStr, LayoutGroup.SecondaryAlignment);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(LayoutGroup, "Change Grid Layout Options");
                LayoutGroup.GridConstraintCount = GridConstraintCount;
                LayoutGroup.GridLayoutAxis = GridLayoutAxis;
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
            UseFullCircle = EditorGUILayout.Toggle("Use Full Circle", LayoutGroup.UseFullCircle);
            if(!UseFullCircle)
            {
                MaxArcAngle = EditorGUILayout.FloatField("Max Arc Angle", LayoutGroup.MaxArcAngle);
            }
            else
            {
                int childCount = LayoutGroup.transform.childCount;
                MaxArcAngle = 360f - 360f / childCount;
            }
            Radius = EditorGUILayout.FloatField("Radius", LayoutGroup.Radius);
            StartAngleOffset = EditorGUILayout.FloatField("Start Angle Offset", LayoutGroup.StartAngleOffset);
            SpiralFactor = EditorGUILayout.FloatField("Spiral Factor", LayoutGroup.SpiralFactor);
            AlignToRadius = EditorGUILayout.Toggle("Align To Radius", LayoutGroup.AlignToRadius);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(LayoutGroup, "Change Radial Layout Options");
                LayoutGroup.UseFullCircle = UseFullCircle;
                LayoutGroup.MaxArcAngle = MaxArcAngle;
                LayoutGroup.Radius = Radius;
                LayoutGroup.StartAngleOffset = StartAngleOffset;
                LayoutGroup.SpiralFactor = SpiralFactor;
                LayoutGroup.AlignToRadius = AlignToRadius;
                shouldRebuild = true;
            }
        }

        if (LayoutGroup.Style != LayoutStyle.Radial)
        {
            EditorGUI.BeginChangeCheck();
            Spacing = EditorGUILayout.FloatField("Spacing", LayoutGroup.Spacing);
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

}
