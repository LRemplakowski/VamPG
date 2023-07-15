//Created by Scriptable Object Database plugin.

using MyLib.EditorTools;
using UnityEditor;

[CustomEditor(typeof(EquippableItemDatabaseFile))]
public class EquippableItemDatabaseInspector : DatabaseInspectorBase<EquippableItemDatabaseFile, ItemBaseSObject>
{
    public override void ReloadGUI()
    {
        pEditorGUI = new EquippableItemDatabaseEditorGUI("EquippableItemDatabase" + pThisDatabase.ID16 + ".Inspector");
        pEditorGUI.InitInspector();
    }
}

public class EquippableItemDatabaseEditorGUI : DatabaseWindowEditor<EquippableItemDatabaseFile, ItemBaseSObject>
{
	public override string EditorName { get { return "Equippable"; } }

	public override System.Type ParentType
    {
        get
        {
            return typeof(ItemDatabaseEditorGUI);
        }
    }

    public EquippableItemDatabaseEditorGUI(string editorPrefsPrefix)
        : base(editorPrefsPrefix) { }
}