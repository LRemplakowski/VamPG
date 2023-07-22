//Created by Scriptable Object Database plugin.
using SunsetSystems.Journal;
using MyLib.EditorTools;
using UnityEditor;

[CustomEditor(typeof(ObjectiveFile))]
public class ObjectiveInspector : DatabaseInspectorBase<ObjectiveFile, Objective>
{
    public override void ReloadGUI()
    {
        pEditorGUI = new ObjectiveEditorGUI("Objective" + pThisDatabase.ID16 + ".Inspector");
        pEditorGUI.InitInspector();
    }
}

public class ObjectiveEditorGUI : DatabaseWindowEditor<ObjectiveFile, Objective>
{
    public override string EditorName { get { return "Objective"; } }
    
    public override System.Type ParentType
    {
        get
        {
            return typeof(QuestEditorGUI);
        }
    }
    
    public ObjectiveEditorGUI(string editorPrefsPrefix)
        : base(editorPrefsPrefix) { }
}