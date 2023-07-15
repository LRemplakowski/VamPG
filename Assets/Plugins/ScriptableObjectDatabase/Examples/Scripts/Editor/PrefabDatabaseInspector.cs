using MyLib.EditorTools;
using System.Threading;
using MyLib.Shared.Database;
using MyLib.EditorTools.Tools;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrefabDatabaseFile))]
public class PrefabDatabaseInspector : DatabaseInspectorBase<PrefabDatabaseFile, GameObject>
{
    public override void ReloadGUI()
    {
        pEditorGUI = new PrefabDatabaseEditorGUI("PrefabDatabase" + pThisDatabase.ID16 + ".Inspector");
        pEditorGUI.InitInspector();
    }
}

public class PrefabDatabaseEditorGUI : DatabaseWindowEditor<PrefabDatabaseFile, GameObject>
{
    private GameObject mNewObject;

    public override string EditorName { get { return "Prefabs"; } }

    public PrefabDatabaseEditorGUI(string editorPrefsPrefix)
        : base(editorPrefsPrefix)
    {
    }

    protected override void DrawSerializedObject(SerializedObject sObject)
    {
        if (GUILayout.Button("Open Prefab"))
            AssetDatabase.OpenAsset(sObject.targetObject);
    }

    public override void DrawDatabaseDetails<A>(Rect rect, IDatabaseFile database)
    {
        if (CurDatabase == null)
            EditorGUILayout.HelpBox("You must first select a database to add Assets to.", MessageType.Info);
        else if (CurDatabase.File == null)
            EditorGUILayout.HelpBox("You must first select a database to add Assets to.", MessageType.Info);
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //Current Asset preview.
            EditorGUILayout.BeginVertical(GUILayout.Width(64f), GUILayout.Height(64f));
            {
                if (mNewObject != null)
                {
#if UNITY_2018_3_OR_NEWER
                    GUILayout.Box(PrefabUtility.GetIconForGameObject(mNewObject), GUILayout.Width(64), GUILayout.Height(64));
#else
                    GUILayout.Box("Select An Asset", GUILayout.Width(64), GUILayout.Height(64));
#endif
                }
                else
                    GUILayout.Box("Select An Asset", GUILayout.Width(64), GUILayout.Height(64));
            }
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            #region Add New Asset

            EditorGUILayout.BeginVertical();
            {
                mNewObject = EditorGUILayout.ObjectField(new GUIContent("", "Click for a list of available Prefabs"), mNewObject, typeof(GameObject), false) as GameObject;
                //if (GUILayout.Button(new GUIContent(mNewObject == null ? "Select Prefab" : mNewObject.name, "Click for a list of available Prefabs"), "ExposablePopupMenu"))
                //PrefabSelector.PrefabSelection(SetPrefab);

                if (GUILayout.Button(new GUIContent("Add Asset",
                        "Adds the selected Asset to the 'Database'.")))
                {
                    if (mNewObject != null)
                    {
                        int key = CurDatabase.AddNew(mNewObject.name, mNewObject);
                        int assetId = mNewObject.GetInstanceID();

                        AddIconToAtlas(null, key);

                        SetDirty(CurDatabase);
                        mNewObject = null;
                    }
                    else
                    {
                        EditorApplication.Beep();
                        EditorUtility.DisplayDialog("No Asset Selected",
                            "Select an asset to add to the 'Database'.",
                            "OK");
                    }
                }
            }
            EditorGUILayout.EndVertical();

            #endregion Add New Asset
        }
    }

    public override int DrawLeftColumn<U>(Rect position)
    {
        //Account for the extra room taken by the Database Deatils
        return base.DrawLeftColumn<U>(new Rect(position.x, position.y, position.width, position.height - 78f));
    }

    protected void SetPrefab(int instanceId)
    {
        mNewObject = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(instanceId), typeof(GameObject)) as GameObject;
    }
}