//Created by Scriptable Object Database plugin.
using MyLib.EditorTools;
using UMA.CharacterSystem;
using UnityEditor;

[CustomEditor(typeof(WardrobeCollectionDatabaseFile))]
public class WardrobeCollectionDatabaseInspector : DatabaseInspectorBase<WardrobeCollectionDatabaseFile, UMAWardrobeCollection>
{
    public override void ReloadGUI()
    {
        pEditorGUI = new WardrobeCollectionDatabaseEditorGUI("WardrobeCollectionDatabase" + pThisDatabase.ID16 + ".Inspector");
        pEditorGUI.InitInspector();
    }
}

public class WardrobeCollectionDatabaseEditorGUI : DatabaseWindowEditor<WardrobeCollectionDatabaseFile, UMAWardrobeCollection>
{
    public override string EditorName { get { return "Wardrobe Collection Database"; } }
    
    public WardrobeCollectionDatabaseEditorGUI(string editorPrefsPrefix)
        : base(editorPrefsPrefix) { }
}