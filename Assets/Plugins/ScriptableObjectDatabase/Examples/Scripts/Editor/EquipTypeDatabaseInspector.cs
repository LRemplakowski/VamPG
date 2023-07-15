//Created by Scriptable Object Database plugin.

using MyLib.EditorTools;
using UnityEditor;

[CustomEditor(typeof(EquipTypeDatabaseFile))]
public class EquipTypeDatabaseInspector : DatabaseInspectorBase<EquipTypeDatabaseFile, EquippableItemTypeSObject>
{
    public override void ReloadGUI()
    {
        pEditorGUI = new EquipTypeDatabaseEditorGUI("EquipTypeDatabase" + pThisDatabase.ID16 + ".Inspector");
        pEditorGUI.InitInspector();
    }
}

public class EquipTypeDatabaseEditorGUI : DatabaseWindowEditor<EquipTypeDatabaseFile, EquippableItemTypeSObject>
{
	public override string EditorName { get { return "Equip Type"; } }

	public override System.Type ParentType
    {
        get
        {
            return typeof(EquippableItemDatabaseEditorGUI);
        }
    }

    public EquipTypeDatabaseEditorGUI(string editorPrefsPrefix)
        : base(editorPrefsPrefix) { }
}