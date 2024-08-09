using MyLib.EditorTools.Tools;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DatabaseTypeCreationWindow : EditorWindow
{
    private string mRuntimeSavePath;
    private string mEditorSavePath;
    private string mDatabaseTypeName = "";
    private string mEditorName = "";

    private float mIndent = 190f;
    private Object mSelectedObject;
    private Object mNewObject;
    private List<System.Type> mTypes = new List<System.Type>();
    private int mCurSelectionIndex = -1;
    private Vector2 mScrollPos = Vector2.zero;

    /// <summary>
    /// Save path for normal scripts.
    /// </summary>
    public string RuntimeSavePath
    {
        get
        {
            if (string.IsNullOrEmpty(mRuntimeSavePath))
                return "Runtime Scripts Save Folder...";

            return mRuntimeSavePath;
        }
        set
        {
            mRuntimeSavePath = value;
        }
    }

    /// <summary>
    /// Save path for editor only scripts
    /// </summary>
    public string EditorSavePath
    {
        get
        {
            if (string.IsNullOrEmpty(mEditorSavePath))
                return "Editor Scripts Save Folder...";

            return mEditorSavePath;
        }
        set
        {
            mEditorSavePath = value;
        }
    }

    [MenuItem("Window/ScriptableObject Database/Database Wizard")]
    public static void OpenDatabaseTypeWindow()
    {
        bool createWindow = false;

        // Get existing open window or if none, make a new one:
        FocusWindowIfItsOpen<DatabaseTypeCreationWindow>();
        if (focusedWindow == null)
            createWindow = true;
        else if (focusedWindow.GetType() != typeof(DatabaseTypeCreationWindow))
            createWindow = true;

        if (createWindow)
        {
            DatabaseTypeCreationWindow window = GetWindow<DatabaseTypeCreationWindow>(false, "Database Type Creator Wizard", true);
            window.position = new Rect(64, 64, 512, 480);
            window.minSize = new Vector2(256, 128);
        }
    }

    public void OnSelectionChange()
    {
        Repaint();
    }

    public void OnDestroy()
    {
        EditorUtility.UnloadUnusedAssetsImmediate();
    }

    private void OnDidOpenScene()
    {
    }

    private void OnEnable()
    {
    }

    /// <summary>
    /// Draws the Database creation GUI
    /// </summary>
    public void OnGUI()
    {
        mScrollPos = GUILayout.BeginScrollView(mScrollPos, false, true, GUILayout.Width(position.width), GUILayout.Height(position.height));
        GUITools.BeginContents();
        {
            GUI.color = Color.gray;
            GUITools.BeginContents();
            {
                GUI.color = Color.white;
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Object Type", "HelpBox", GUILayout.Width(mIndent));
                    mNewObject = EditorGUILayout.ObjectField(mSelectedObject, typeof(ScriptableObject), false);
                }
                GUILayout.EndHorizontal();

                if (mNewObject != mSelectedObject || (mSelectedObject != null && mTypes.Count == 0))
                {
                    mCurSelectionIndex = -1;
                    mTypes.Clear();

                    if (mNewObject != null)
                    {
                        mTypes.AddRange(CustomEditorTools.GetAllBaseTypes(mNewObject, false));
                        mCurSelectionIndex = 0;
                    }

                    mTypes.Reverse();

                    mSelectedObject = mNewObject;
                }

                GUILayout.BeginHorizontal();
                {
                    if (mTypes.Count > 0)
                    {
                        GUITools.BeginContents();
                        GUILayout.BeginVertical(GUILayout.Width(mIndent - 8f));
                        {

                            for (int i = 0; i < mTypes.Count; i++)
                            {
                                GUI.color = i >= mCurSelectionIndex ? Color.green : Color.gray;
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label(i + "", "HelpBox", GUILayout.Width(18f));
                                    if (GUILayout.Button(mTypes[i].Name, "HelpBox"))
                                        mCurSelectionIndex = i;
                                }
                                GUILayout.EndHorizontal();
                            }
                            GUI.color = Color.white;
                        }
                        GUILayout.EndVertical();
                        GUITools.EndContents();
                        EditorGUILayout.HelpBox("The buttons to the left represent the dependencies of the selected ScriptableObject," +
                            " The top item is the base class right above the ScriptableObject class. Selecting the top option allows a database" +
                            " to hold and create all the Sub class types of that ScriptableObject.", MessageType.Info);
                    }
                    else
                    {
                        GUILayout.Space(mIndent + 8f);
                        EditorGUILayout.HelpBox("Select a ScriptableObject you wish to make a database for.", MessageType.Info);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUITools.EndContents();

            GUITools.DrawHorizontalSeparator(4, Color.gray);

            GUI.color = Color.gray;
            GUITools.BeginContents();
            {
                GUI.color = Color.white;
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Database Type Class Name", "HelpBox", GUILayout.Width(mIndent));
                    mDatabaseTypeName = GUILayout.TextField(mDatabaseTypeName);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(mIndent + 8f);
                    if (!mDatabaseTypeName.All(char.IsLetterOrDigit))
                        EditorGUILayout.HelpBox("The Database name may contain unsupported characters." +
                            " Do not use '.', '/', or spaces in this name as it will be used as class names in scripts." +
                            " Disreguard if using '_' and getting this error.", MessageType.Error);
                    else
                        EditorGUILayout.HelpBox("This name is used to create the classes used for the database. Leaving the Name field" +
                            " empty will result in the ScriptableObjects Type being used to name files and classes." +
                            " Do not use '.', '/', or spaces in this name as it will be used as class names in scripts.", MessageType.Info);
                }
                GUILayout.EndHorizontal();
            }
            GUITools.EndContents();

            GUITools.DrawHorizontalSeparator(4, Color.gray);

            GUI.color = Color.gray;
            GUITools.BeginContents();
            {
                GUI.color = Color.white;
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Database Editor Name", "HelpBox", GUILayout.Width(mIndent));
                    mEditorName = GUILayout.TextArea(mEditorName);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(mIndent + 8f);
                    EditorGUILayout.HelpBox("A short name for the database, to be displayed on the editor button" +
                        " as well as used in the Editor Prefs to save editor state.", MessageType.Info);
                }
                GUILayout.EndHorizontal();
            }
            GUITools.EndContents();

            GUITools.DrawHorizontalSeparator(4, Color.gray);

            GUI.color = Color.gray;
            GUITools.BeginContents();
            {
                GUI.color = Color.white;
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Runtime Save Folder", "HelpBox", GUILayout.Width(mIndent));
                    GUILayout.TextArea(RuntimeSavePath);
                
                    if (GUILayout.Button(EditorGUIUtility.FindTexture("Folder Icon"), "HelpBox", GUILayout.Width(20f), GUILayout.Height(20f)))
                    {
                        RuntimeSavePath = EditorUtility.SaveFolderPanel("Select a folder to save Runtime scripts to.",
                            string.IsNullOrEmpty(mRuntimeSavePath) ? Application.dataPath : mRuntimeSavePath, "");
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(mIndent + 8f);
                    EditorGUILayout.HelpBox("The Runtime sripts will be saved to this location.", MessageType.Info);
                }
                GUILayout.EndHorizontal();
            }
            GUITools.EndContents();

            GUITools.DrawHorizontalSeparator(4, Color.gray);

            GUI.color = Color.gray;
            GUITools.BeginContents();
            {
                GUI.color = Color.white;
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Editor Save Folder", "HelpBox", GUILayout.Width(mIndent));
                    GUILayout.TextArea(EditorSavePath);

                    if (GUILayout.Button(EditorGUIUtility.FindTexture("Folder Icon"), "HelpBox", GUILayout.Width(20f), GUILayout.Height(20f)))
                    {
                        EditorSavePath = EditorUtility.SaveFolderPanel("Select a folder to save Editor scripts to.",
                            string.IsNullOrEmpty(mEditorSavePath) ? Application.dataPath : mEditorSavePath, "");
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(mIndent + 8f);
                    EditorGUILayout.HelpBox("The Editor sripts will be saved to this location.", MessageType.Info);
                }
                GUILayout.EndHorizontal();
            }
            GUITools.EndContents();
        }
        GUITools.EndContents();

        GUI.color = Color.green;
        if (GUILayout.Button("Create New Database Type", "LargeButton"))
        {
            bool result = CreateDatabaseType();

            if (!result)
                Debug.Log("Error creating database, please validate information.");
            else
                Debug.Log("Successfully created database files.");
        }

        GUI.color = Color.white;
        GUILayout.EndScrollView();
    }

    /// <summary>
    /// Validates input creation data, and created a database type.
    /// </summary>
    /// <returns></returns>
    public bool CreateDatabaseType()
    {
        if (mCurSelectionIndex == -1 || mTypes.Count == 0)
        {
            Debug.Log("No Scriptable Object Base Class Selected.");
            return false;
        }

        if (string.IsNullOrEmpty(mRuntimeSavePath))
        {
            Debug.Log("No Runtime Save path specified. Please set a Runtime Save Path");
            return false;
        }

        if (string.IsNullOrEmpty(mEditorSavePath))
        {
            Debug.Log("No Editor Save path specified. Please set a Editor Save Path");
            return false;
        }

        System.Type baseType = mTypes[mCurSelectionIndex];

        string objectType = baseType.Name;
        string objectNamespace = baseType.Namespace;

        if (string.IsNullOrEmpty(mDatabaseTypeName))
        {
            Debug.Log("The 'Database Type Name' is not set. This name is used to create the classes used for the database." +
                " The ScriptableObjects Type will be used to name files and classes.");

            mDatabaseTypeName = objectType;
        }

        if (string.IsNullOrEmpty(mEditorName))
        {
            Debug.Log("Editor Name not specified. Will use the type as the Editor name" +
                " this can be changed by modifying the script file that is created, and changing the" +
                " EditorName property on the '" + mDatabaseTypeName + "EditorGUI' class.");

            mEditorName = mDatabaseTypeName;
        }

        //string databaseText = "";
        //using (Stream strm = @Assembly.GetExecutingAssembly().GetManifestResourceStream("ScriptableObjectDatabaseEditor.Resources.DatabaseFileTemplate.txt"))
        //{
        //databaseText = System.Text.Encoding.UTF8.GetString((strm as MemoryStream).ToArray());
        //}

        TextAsset asset = EditorGUIUtility.Load("ScriptableObjectDatabase/DatabaseFileTemplate.txt") as TextAsset;
        string databaseText = asset.text;

        databaseText = databaseText.Replace("'databaseName'", mDatabaseTypeName);
        databaseText = databaseText.Replace("'objectType'", objectType);

        if (objectNamespace != null && objectNamespace.Length > 0)
            databaseText = databaseText.Replace("'targetNamespace'", objectNamespace);
        else
            databaseText = databaseText.Replace("using 'targetNamespace';", "");

        StreamWriter writer = new StreamWriter(mRuntimeSavePath + "/" + mDatabaseTypeName + "File.cs", false);
        writer.Write(databaseText);
        writer.Close();
        writer.Dispose();

        //string editorText = "";
        //using (Stream strm = @Assembly.GetExecutingAssembly().GetManifestResourceStream("ScriptableObjectDatabaseEditor.Resources.DatabaseInspectorTemplate.txt"))
        //{
        //    editorText = System.Text.Encoding.UTF8.GetString((strm as MemoryStream).ToArray());
        //}

        asset = EditorGUIUtility.Load("ScriptableObjectDatabase/DatabaseInspectorTemplate.txt") as TextAsset;
        string editorText = asset.text;

        editorText = editorText.Replace("'databaseName'", mDatabaseTypeName);
        editorText = editorText.Replace("'objectType'", objectType);
        editorText = editorText.Replace("'editorName'", mEditorName);

        if (objectNamespace != null && objectNamespace.Length > 0)
            editorText = editorText.Replace("'targetNamespace'", objectNamespace);
        else
            editorText = editorText.Replace("using 'targetNamespace';", "");

        writer = new StreamWriter(mEditorSavePath + "/" + mDatabaseTypeName + "Inspector.cs", false);
        writer.Write(editorText);
        writer.Close();
        writer.Dispose();

        AssetDatabase.Refresh();

        return true;
    }
}