//Created by Scriptable Object Database plugin.
using MyLib.EditorTools;
using UnityEditor;
using SunsetSystems.Inventory.Data;

[CustomEditor(typeof(QuestFile))]
public class ItemInspector : DatabaseInspectorBase<ItemFile, BaseItem>
{
    public override void ReloadGUI()
    {
        pEditorGUI = new ItemEditorGUI("BaseItem" + pThisDatabase.ID16 + ".Inspector");
        pEditorGUI.InitInspector();
    }
}

public class ItemEditorGUI : DatabaseWindowEditor<ItemFile, BaseItem>
{
    public override string EditorName { get { return "BaseItem"; } }

    public override System.Type ParentType
    {
        get
        {
            return null;
        }
    }

    public ItemEditorGUI(string editorPrefsPrefix)
        : base(editorPrefsPrefix) { }
}