//Created by Scriptable Object Database plugin.

using MyLib.EditorTools;
using UnityEditor;

[CustomEditor(typeof(ItemCategoryDatabaseFile))]
public class ItemCategoryDatabaseInspector : DatabaseInspectorBase<ItemCategoryDatabaseFile, ItemCategorySObject>
{
    public override void ReloadGUI()
    {
        pEditorGUI = new ItemCategoryDatabaseEditorGUI("ItemCategoryDatabase" + pThisDatabase.ID16 + ".Inspector");
        pEditorGUI.InitInspector();
    }
}

public class ItemCategoryDatabaseEditorGUI : DatabaseWindowEditor<ItemCategoryDatabaseFile, ItemCategorySObject>
{
	public override string EditorName { get { return "Item Category"; } }

	public override System.Type ParentType
    {
        get
        {
            return typeof(ItemDatabaseEditorGUI);
        }
    }

    public ItemCategoryDatabaseEditorGUI(string editorPrefsPrefix)
        : base(editorPrefsPrefix) { }
}