//Created by Scriptable Object Database plugin.
using SunsetSystems.Entities.Characters;
using MyLib.EditorTools;
using UnityEditor;

[CustomEditor(typeof(CreatureConfigFile))]
public class CreatureConfigInspector : DatabaseInspectorBase<CreatureConfigFile, CreatureConfig>
{
    public override void ReloadGUI()
    {
        pEditorGUI = new CreatureConfigEditorGUI("CreatureConfig" + pThisDatabase.ID16 + ".Inspector");
        pEditorGUI.InitInspector();
    }
}

public class CreatureConfigEditorGUI : DatabaseWindowEditor<CreatureConfigFile, CreatureConfig>
{
    public override string EditorName { get { return "Creatures"; } }
    
    public override System.Type ParentType
    {
        get
        {
            return null;
        }
    }
    
    public CreatureConfigEditorGUI(string editorPrefsPrefix)
        : base(editorPrefsPrefix) { }
}