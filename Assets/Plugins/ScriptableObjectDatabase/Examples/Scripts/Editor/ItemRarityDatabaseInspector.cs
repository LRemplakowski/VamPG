//Created by Scriptable Object Database plugin.

using MyLib.EditorTools;
using UnityEditor;

[CustomEditor(typeof(ItemRarityDatabaseFile))]
public class ItemRarityDatabaseInspector : DatabaseInspectorBase<ItemRarityDatabaseFile, ItemRaritySObject>
{
    public override void ReloadGUI()
    {
        pEditorGUI = new ItemRarityDatabaseEditorGUI("ItemRarityDatabase" + pThisDatabase.ID16 + ".Inspector");
        pEditorGUI.InitInspector();
    }
}

public class ItemRarityDatabaseEditorGUI : DatabaseWindowEditor<ItemRarityDatabaseFile, ItemRaritySObject>
{
    public override System.Type ParentType
    {
        get
        {
            return typeof(ItemDatabaseEditorGUI);
        }
    }

    public override string EditorName { get { return "Item Rarity"; } }

    public ItemRarityDatabaseEditorGUI(string editorPrefsPrefix)
        : base(editorPrefsPrefix) { }
}