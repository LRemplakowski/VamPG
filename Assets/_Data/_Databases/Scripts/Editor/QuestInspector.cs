//Created by Scriptable Object Database plugin.
using SunsetSystems.Journal;
using MyLib.EditorTools;
using UnityEditor;

[CustomEditor(typeof(QuestFile))]
public class QuestInspector : DatabaseInspectorBase<QuestFile, Quest>
{
    public override void ReloadGUI()
    {
        pEditorGUI = new QuestEditorGUI("Quest" + pThisDatabase.ID16 + ".Inspector");
        pEditorGUI.InitInspector();
    }
}

public class QuestEditorGUI : DatabaseWindowEditor<QuestFile, Quest>
{
    public override string EditorName { get { return "Quest"; } }
    
    public override System.Type ParentType
    {
        get
        {
            return null;
        }
    }
    
    public QuestEditorGUI(string editorPrefsPrefix)
        : base(editorPrefsPrefix) { }
}