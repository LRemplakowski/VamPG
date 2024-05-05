using MyLib.EditorTools;
using MyLib.EditorTools.Tools;
using UnityEditor;
using UnityEngine;

public class AssetEditorWindow : EditorWindow
{    
    protected DatabaseWindowEditor pCurEditor;

    private Vector2 mDatabaseScrollPosition;

    private int mCurEditorIndex = -1;
    private int mCurAssetId32 = -1;
    private AddDatabaseButtonTypePopup mPopUp;

    public string Name { get { return "AssetEditorPrefs"; } }

    /// <summary>
    /// The currently selected editors index.
    /// </summary>
    public int CurEditorIndex
    {
        get
        {
            if (mCurEditorIndex == -1)
                mCurEditorIndex = EditorPrefs.GetInt(Name + ".CurEditor", -1);

            return mCurEditorIndex;
        }

        set
        {
            mCurEditorIndex = value;
            EditorPrefs.SetInt(Name + ".CurEditor", value);
        }
    }

    /// <summary>
    /// The current 32 bit asset ID.
    /// </summary>
    public int CurAssetID32
    {
        get
        {
            if (mCurAssetId32 == -1)
                mCurAssetId32 = EditorPrefs.GetInt(mPopUp.editorInstances[CurEditorIndex].PrefsName + ".CurAssetIndex", -1);

            return mCurAssetId32;
        }

        set
        {
            mCurAssetId32 = value;
            EditorPrefs.SetInt(mPopUp.editorInstances[CurEditorIndex].PrefsName + ".CurAssetIndex", value);
        }
    }

    [MenuItem("Window/ScriptableObject Database/Editor Window")]
    public static void AssetEditorActivate()
    {
        bool createWindow = false;

        // Get existing open window or if none, make a new one:
        EditorWindow.FocusWindowIfItsOpen<AssetEditorWindow>();
        if (EditorWindow.focusedWindow == null)
            createWindow = true;
        else if (EditorWindow.focusedWindow.GetType() != typeof(AssetEditorWindow))
            createWindow = true;

        if (createWindow)
        {
            AssetEditorWindow window = EditorWindow.GetWindow<AssetEditorWindow>(false, "Asset Database Editor", true);
            window.position = new Rect(64, 64, 428, 428);
            window.minSize = new Vector2(100, 100);
        }
    }

    /// <summary>
    /// Initialization of editor window.
    /// </summary>
    public virtual void Init()
    {
        mPopUp = new AddDatabaseButtonTypePopup(this);
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
        Init();
    }

    /// <summary>
    /// Draw the currently selected editor.
    /// </summary>
    public void OnGUI()
    {
        if (pCurEditor == null)
        {
            if (CurEditorIndex != -1 && mPopUp.editorInstances.Length > CurEditorIndex)
            {
                pCurEditor = mPopUp.editorInstances[CurEditorIndex];
            }
        }

        if (pCurEditor != null && pCurEditor.Enabled == false)
        {
            pCurEditor = null;
            CurEditorIndex = -1;
        }

        #region Draw Database Type Selection

        int count = 0;
        GUILayout.BeginHorizontal();
        {
            for (int i = 0; i < mPopUp.editorInstances.Length; i++)
            {
                DatabaseWindowEditor editor = mPopUp.editorInstances[i];

                if (!editor.Enabled)
                    continue;

                if (CurEditorIndex != i)
                    GUI.color = new Color(0.75f, 0.75f, 0.75f, 1);
                
                if (GUILayout.Button(editor.EditorName, 
                    (count == 0 ? "ButtonLeft" : "ButtonMid"), GUILayout.Height(24f)))
                {
                    pCurEditor = editor;
                    CurEditorIndex = i;
                }

                GUI.color = Color.white;

                count++;
            }

            if (count == 0)
            {
                GUILayout.Label("Click the button to the right to enable and disable editors", "HelpBox", GUILayout.Height(24f));
                //GUILayout.Space(position.width - 24f);
            }

            GUILayout.Space(24f);
            Rect lRect = GUILayoutUtility.GetLastRect();
            if (GUI.Button(new Rect(lRect.x, lRect.y + 3f, 24f, 24f), EditorGUIUtility.IconContent("GameManager Icon", ""), "ButtonRight"))
                PopupWindow.Show(new Rect(lRect.x, lRect.y + 3f, 0, 24f), mPopUp);
        }
        GUILayout.EndHorizontal();

        #endregion Draw Database Type Selection

        if (pCurEditor != null)
        {
            pCurEditor.DrawHeader();

            GUITools.DrawHorizontalSeparator(0, 0, (int)position.width, 4, Color.gray);

            #region Draw Body

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    mDatabaseScrollPosition = GUILayout.BeginScrollView(mDatabaseScrollPosition, GUILayout.Width(230));
                    {
                        pCurEditor.DrawLeftColumn(new Rect(position.x, position.y, 200, position.height - 80));
                    }
                    GUILayout.EndScrollView();

                    GUITools.DrawVerticalSeparator(1, -3, (int)position.height - 60, 4, Color.gray);

                    pCurEditor.DrawRightColumn(new Rect(position.x, position.y, position.width - 236f, position.height - 80));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            pCurEditor.ProcessDragAssets();
        }
        #endregion Draw Body
    }
}