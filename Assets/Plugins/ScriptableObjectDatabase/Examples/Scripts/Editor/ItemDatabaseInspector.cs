//Created by Scriptable Object Database plugin.

using MyLib.EditorTools;
using UnityEditor;
using MyLib.Shared.Database;
using UnityEngine;
using MyLib.EditorTools.Tools;
using System;

[CustomEditor(typeof(ItemDatabaseFile))]
public class ItemDatabaseInspector : DatabaseInspectorBase<ItemDatabaseFile, ItemBaseSObject>
{
    public override void ReloadGUI()
    {
        pEditorGUI = new ItemDatabaseEditorGUI("ItemDatabase" + pThisDatabase.ID16 + ".Inspector");
        pEditorGUI.InitInspector();
    }
}

public class ItemDatabaseEditorGUI : DatabaseWindowEditor<ItemDatabaseFile, ItemBaseSObject>
{
    public override string EditorName { get { return "Items"; } }

    public ItemDatabaseEditorGUI(string editorPrefsPrefix)
        : base(editorPrefsPrefix) { }
}
