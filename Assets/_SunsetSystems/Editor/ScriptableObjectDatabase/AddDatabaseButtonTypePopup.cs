using MyLib.EditorTools.Tools;
using MyLib.EditorTools;
using System.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AddDatabaseButtonTypePopup : PopupWindowContent
{
    public DatabaseWindowEditor[] editorInstances;

    private Type[] mDatabaseTypes;
    private string[] mDatabaseTypeStrings;
    
    private Vector2 mScrollPos = Vector2.zero;
    private EditorWindow mCallingWindow;

    public AddDatabaseButtonTypePopup(EditorWindow callingWindow)
    {
        mCallingWindow = callingWindow;

        if (mDatabaseTypes == null || mDatabaseTypes.Length == 0)
        {
            Type[] derivedTypes = CustomEditorTools.GetAllDerivedTypes(AppDomain.CurrentDomain, typeof(DatabaseWindowEditor));
            mDatabaseTypeStrings = new string[derivedTypes.Length];
            mDatabaseTypes = new Type[derivedTypes.Length];
            editorInstances = new DatabaseWindowEditor[derivedTypes.Length];

            for (int index = 0; index < derivedTypes.Length; index++)
            {
                mDatabaseTypeStrings[index] = derivedTypes[index].FullName;
                mDatabaseTypes[index] = derivedTypes[index];
                editorInstances[index] = Activator.CreateInstance(derivedTypes[index], new object[] { "AssetEditorPrefs" }) as DatabaseWindowEditor;
            }
        }
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(256, Mathf.Min(23f + (mDatabaseTypes == null ? 23f : mDatabaseTypes.Length * 23f), 168));
    }

    public override void OnGUI(Rect rect)
    {
        mScrollPos = GUILayout.BeginScrollView(mScrollPos);
        
        if (GUILayout.Button("Create New Database Type", "HelpBox", GUILayout.Width(220f)))
            DatabaseTypeCreationWindow.OpenDatabaseTypeWindow();

        for (int i = 0; i < mDatabaseTypes.Length; i++)
        {
            bool enabled = editorInstances[i].Enabled;

            GUI.color = enabled ? Color.green : Color.red;
            if (GUILayout.Button((editorInstances[i].EditorName) + "", "HelpBox", GUILayout.Width(220f)))
            {
                editorInstances[i].Enabled = !enabled;
                mCallingWindow.Repaint();
            }
        }
        GUI.color = Color.white;
        GUILayout.EndScrollView();
    }
}