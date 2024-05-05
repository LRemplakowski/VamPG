using MyLib.EditorTools.Tools;
using MyLib.Shared.Database;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MyLib.EditorTools
{
    public abstract class DatabaseWindowEditor<S, U> : DatabaseWindowEditor
        where S : ScriptableObject, IDatabaseFile
        where U : UnityEngine.Object
    {
        private Editor mEditor;

        public DatabaseWindowEditor(string editorPrefsPrefix)
        {
            InitWindow(editorPrefsPrefix);
        }

        public override void DrawHeader()
        {
            DrawHeader<S>();
        }

        public override void DrawLeftColumn(Rect position)
        {
            DrawLeftColumn<U>(position);
        }

        protected override void DrawSerializedObject(SerializedObject sObject)
        {
            if (sObject == null || sObject.targetObject == null)
                return;

            if (mEditor == null || mEditor.target != sObject.targetObject)
                mEditor = Editor.CreateEditor(sObject.targetObject);

            mEditor.OnInspectorGUI();

            sObject.Update();
        }
    }

    /// <summary>
    /// Base database window editor used to display database contents.
    /// </summary>
    public abstract class DatabaseWindowEditor
    {
        protected string pSaveName;
        protected Vector2 pDatabaseScrollPosition = Vector2.zero;
        protected Vector2 pAssetScrollPosition = Vector2.zero;

        private bool mLoaded = false;
        private bool mEnabled = true;

        private IDatabaseFile mFile;
        private short mFileKey16 = -1;
        private DatabaseAsset mAsset;
        private short mAssetKey16 = -1;

        private string[] mDatabaseNames;
        private ScriptableObject[] mDatabases;
        private int mDatabaseSelector = 0;

        private Object[] mDragObjects;
        private SerializedObject mSerializedObject;

        private bool mInitialized = false;
        private int mTypeSelector = 0;
        private System.Type[] mTypes;
        private string[] mTypeStrings;
        private string mSearchFilter;

        public bool Enabled
        {
            get
            {
                if (!mLoaded)
                {
                    mLoaded = true;
                    mEnabled = EditorPrefs.GetBool(pSaveName + "enabled", true);
                }

                return mEnabled;
            }

            set
            {
                mEnabled = value;
                EditorPrefs.SetBool(pSaveName + "enabled", value);
            }
        }

        /// <summary>
        /// Is the user searching at the moment?
        /// </summary>
        public bool IsSearching
        {
            get { return mSearchFilter != "" && mSearchFilter != "Search..."; }
        }

        /// <summary>
        /// Used to save preference data.
        /// </summary>
        public string PrefsName { get { return pSaveName; } }

        /// <summary>
        /// The name of this editor.
        /// </summary>
        public virtual string EditorName { get { return "Databases"; } }

        /// <summary>
        /// The currently selected database.
        /// </summary>
        public virtual IDatabaseFile CurDatabase
        {
            get
            {
                if (mFileKey16 == -1)
                {
                    mFileKey16 = (short)EditorPrefs.GetInt(pSaveName + "fileID", -1);

                    if (mFileKey16 == -1)
                        return null;
                }

                if (mFile == null || mFile.ID16 != mFileKey16)
                {
                    mFile = DatabaseManager.GetDatabase(mFileKey16);

                    if (mFile == null)
                    {
                        mFileKey16 = -1;
                        EditorPrefs.DeleteKey(pSaveName + "fileID");
                        EditorPrefs.SetInt(pSaveName + "fileID", -1);
                    }
                }

                if (mFile != null && mFile.File == null)
                {
                    mFileKey16 = -1;
                    EditorPrefs.DeleteKey(pSaveName + "fileID");
                    EditorPrefs.SetInt(pSaveName + "fileID", -1);
                    mFile = null;
                }

                return mFile;
            }

            set
            {
                mFile = value;

                if (mFile == null)
                    mFileKey16 = -1;
                else
                    mFileKey16 = mFile.ID16;

                EditorPrefs.SetInt(pSaveName + "fileID", mFileKey16);
            }
        }

        /// <summary>
        /// The currently selected asset.
        /// </summary>
        public virtual DatabaseAsset CurAsset
        {
            get
            {
                if (mAssetKey16 == -1)
                {
                    mAssetKey16 = (short)EditorPrefs.GetInt(pSaveName + "assetID", -1);

                    if (mAssetKey16 == -1)
                        return null;
                }

                if (mAsset == null || (mAsset.AssetKey16 != mAssetKey16 && CurDatabase.DatabaseData.ContainsKey(mAssetKey16)))
                {
                    mAsset = CurDatabase.DatabaseData.GetAsset(mAssetKey16);

                    if (mAsset == null)
                    {
                        mAssetKey16 = -1;
                        EditorPrefs.SetInt(pSaveName + "assetID", -1);
                    }
                }

                if (mAsset != null && mAsset.AssetObject == null)
                {
                    mAssetKey16 = -1;
                    EditorPrefs.SetInt(pSaveName + "assetID", -1);
                    mAsset = null;
                }

                return mAsset;
            }

            set
            {
                mAsset = value;

                if (mAsset == null)
                    mAssetKey16 = -1;
                else
                    mAssetKey16 = mAsset.AssetKey16;

                EditorPrefs.SetInt(pSaveName + "assetID", mAssetKey16);
            }
        }

        protected void SetDirty(IDatabaseFile database)
        {
            DatabaseManager.SetDirty(database);
        }

        public DatabaseWindowEditor()
        {
        }

        /// <summary>
        /// Initialize the editor as a window.
        /// </summary>
        /// <param name="editorPrefsPrefix">The preference save prefix.</param>
        public void InitWindow(string editorPrefsPrefix)
        {
            pSaveName = editorPrefsPrefix + "." + EditorName;

            string[] appPath = Application.dataPath.Split('/');
            string projectName = appPath[appPath.Length - 2];
            pSaveName = projectName + "." + pSaveName;
        }

        /// <summary>
        /// Initialize the editor as an inspector.
        /// </summary>
        public void InitInspector()
        {
            string[] appPath = Application.dataPath.Split('/');
            string projectName = appPath[appPath.Length - 2];
            pSaveName = projectName + "." + pSaveName;
        }

        /// <summary>
        /// Override to draw a custom header.
        /// </summary>
        public abstract void DrawHeader();

        /// <summary>
        /// Draws the header using a certain database file type.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        public virtual void DrawHeader<S>()
            where S : ScriptableObject, IDatabaseFile
        {
            VerifyAssetData();

            GUITools.DrawHeader("Selected Database");

            GUILayout.BeginHorizontal();
            {
                if (!mInitialized)
                    GetDatabases<S>();

                if (CurDatabase != null && CurDatabase.File != null)
                {
                    GUILayout.Space(10f);
                    Rect buttonrect = GUILayoutUtility.GetLastRect();
                    buttonrect.y += 4f;
                    GUITools.DrawObjectButton(buttonrect, CurDatabase.File);
                }

                mDatabaseSelector = DrawSelectDatabaseGUI<S>(mDatabaseSelector, mDatabaseNames);

                if (CurDatabase != null)
                {
                    if (CurDatabase.File != null)
                    {
                        GUI.color = Color.red;
                        if (GUILayout.Button(new GUIContent("X", "Deletes the 'Database' from Disc."),
                            GUILayout.Width(18f)))
                        {
                            EditorApplication.Beep();
                            if (EditorUtility.DisplayDialog("Delete Database '" + CurDatabase.File.name + "'",
                                "Are you sure you wish to delete the 'Database' and the files associated with the 'Database'?",
                                "Confirm",
                                "Cancel"))
                            {
                                DeleteSelectedDatabase();
                            }
                        }
                    }
                }

                GUI.color = Color.green;
                if (GUILayout.Button(new GUIContent("Create", "Creates a new 'Database'."),
                            GUILayout.Width(82f)))
                {
                    CreateNewDatabase<S>("NewDatabase", DatabaseManager.GetUniqueDatabaseId());
                }
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws inspector for the selected database.
        /// </summary>
        /// <typeparam name="S">The databases type.</typeparam>
        /// <param name="curSelection">The current selection.</param>
        /// <param name="strings">A list of strings that represent the different databases.</param>
        /// <returns></returns>
        protected int DrawSelectDatabaseGUI<S>(int curSelection, string[] strings)
            where S : ScriptableObject, IDatabaseFile
        {
            GUILayout.Space(8f);

            if (strings != null)
            {
                if (CurDatabase != null)
                    strings[0] = CurDatabase.File.name;
                else
                    strings[0] = "Select a Database";
            }

            GUILayout.BeginVertical();
            GUILayout.Space(4f);
            curSelection = EditorGUILayout.Popup(curSelection, strings, "ExposablePopupMenu");
            GUILayout.Space(4f);
            GUILayout.EndVertical();

            if (curSelection > 0)
            {
                int instanceID = mDatabases[curSelection - 1].GetInstanceID();

                CurDatabase = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(instanceID), typeof(S)) as IDatabaseFile;
                CurAsset = null;
                GetDatabases<S>();

                curSelection = 0;
            }

            return curSelection;
        }

        /// <summary>
        /// Deletes the selected database.
        /// </summary>
        public virtual void DeleteSelectedDatabase()
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(CurDatabase.File));
            CurDatabase = null;
            DatabaseManager.LoadDatabaseList();
        }

        /// <summary>
        /// Override to draw a custom Left column of the editor
        /// </summary>
        /// <param name="position"></param>
        public abstract void DrawLeftColumn(Rect position);

        /// <summary>
        /// Draws the left column of the editor that contains the list of database assets,
        /// and asset creation tools.
        /// </summary>
        /// <typeparam name="U">The type of asset the database uses.</typeparam>
        /// <param name="position">The offset pizel position of the left column.</param>
        /// <returns>The currently seleced database.</returns>
        public virtual int DrawLeftColumn<U>(Rect position)
            where U : UnityEngine.Object
        {
            int selection = -1;

            EditorGUILayout.BeginVertical(GUILayout.Height(position.height), GUILayout.Width(position.width));
            {
                GUITools.DrawHeader("Database Details");

                GUITools.BeginContents();
                {
                    if (CurDatabase != null)
                    {
                        if (CurDatabase.File)
                        {
                            DrawDatabaseDetails<U>(position, CurDatabase);
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Select/Create a 'Database' above to edit.", MessageType.Info);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Select/Create a 'Database' above to edit.", MessageType.Info);
                    }
                }
                GUITools.EndContents();

                //GUITools.DrawHorizontalSeparator(0, 0, (int)position.width + 16, 4, Color.gray);

                GUITools.DrawHeader("Asset List");
                mSearchFilter = GUITools.SearchBar(mSearchFilter, null, IsSearching);

                if (string.IsNullOrEmpty(mSearchFilter))
                    mSearchFilter = "Search...";

                GUITools.BeginContents();
                if (CurDatabase != null)
                {
                    selection = GUIAssetButtonList(CurDatabase, mAssetKey16, position.width - 12f, IsSearching ? mSearchFilter : "");

                    GUI.color = Color.white;

                    if (selection >= 0)
                    {
                        CurAsset = CurDatabase.DatabaseData.GetAsset((short)selection);
                    }
                    else if (selection == -2)
                    {
                        CurAsset = null;
                    }
                }
                else
                    EditorGUILayout.HelpBox("Select/Create a 'Database' above to edit.", MessageType.Info);
                GUITools.EndContents();
            }
            EditorGUILayout.EndVertical();
            mDragObjects = AssetTools.CheckAssetDrag<U>();

            return selection;
        }

        /// <summary>
        /// Draws the selected database details.
        /// </summary>
        /// <typeparam name="U">The asset type the database stores.</typeparam>
        /// <param name="rect">The pixel size of the details list.</param>
        /// <param name="database">The database to draw details for.</param>
        public virtual void DrawDatabaseDetails<U>(Rect rect, IDatabaseFile database)
            where U : UnityEngine.Object
        {
            GUILayout.Label("Database ID: " + database.ID16, "HelpBox");
            GUILayout.Label("Contains | " + database.DatabaseData.NumOfEntries + " | Objects", "HelpBox");

            if (mTypes == null)
            {
                mTypes = CustomEditorTools.GetAllDerivedObjectTypes<U>(System.AppDomain.CurrentDomain);

                mTypeStrings = new string[mTypes.Length + 1];

                string[] path;
                mTypeStrings[0] = "Select an Object to Add.";
                for (int i = 1; i <= mTypes.Length; i++)
                {
                    path = mTypes[i - 1].ToString().Split('.');
                    mTypeStrings[i] = path[path.Length - 1];
                }

                mTypeSelector = 0;
            }

            bool createdAsset = false;

            GUILayout.BeginHorizontal();
            {
                mTypeSelector = EditorGUILayout.Popup(mTypeSelector, mTypeStrings, "ExposablePopupMenu");
                if (mTypeSelector > 0 && GUILayout.Button(new GUIContent("+", "Add the monobehaviour to this GameObject."), "OL Plus", GUILayout.Width(16f)))
                {
                    createdAsset = true;
                    U file = ScriptableObject.CreateInstance(mTypes[mTypeSelector - 1]) as U;
                    file.name = mTypes[mTypeSelector - 1].Name;
                    AssetDatabase.AddObjectToAsset(file, CurDatabase.File);
                    AddIconToAtlas(null, CurDatabase.AddNew(file.name, file));
                    mTypeSelector = 0;
                }
            }
            GUILayout.EndHorizontal();

            if (createdAsset)
            {
                DatabaseManager.SetDirty(CurDatabase);
                CurAsset = null;
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(CurDatabase.File));
            }
        }

        /// <summary>
        /// Draws the selected assets information.
        /// </summary>
        /// <param name="position">The size in pixels of the right column.</param>
        public virtual void DrawRightColumn(Rect position)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width),
                GUILayout.Height(position.height));
            {
                #region Asset Name

                GUITools.DrawHeader("Selected Asset Data");
                if (CurAsset != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(58f);

                        Rect rect = GUILayoutUtility.GetLastRect();
                        rect.width = 18f;
                        rect.height = 18f;
                        GUITools.DrawObjectButton(rect, CurAsset.AssetObject);
                        rect.x += 18f;

                        int assetIndex = CurDatabase.DatabaseData.IndexOf(CurAsset);
                        GUI.color = assetIndex > 0 ? Color.white : Color.gray;
                        if (GUI.Button(rect, "▲", "HelpBox"))
                            CurDatabase.DatabaseData.Shift(assetIndex, true);
                        rect.x += 18f;

                        GUI.color = assetIndex < CurDatabase.DatabaseData.NumOfEntries - 1 ? Color.white : Color.gray;
                        if (GUI.Button(rect, "▼", "HelpBox"))
                            CurDatabase.DatabaseData.Shift(assetIndex, false);
                        GUI.color = Color.white;

                        string newTileName = EditorGUILayout.TextField(CurAsset.Name);

                        GUILayout.Space(18f);

                        if (CurAsset.Name != newTileName)
                        {
                            CurAsset.Name = newTileName;
                            if (AssetDatabase.IsSubAsset(CurAsset.AssetObject))
                                CurAsset.AssetObject.name = newTileName;
                            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(CurDatabase.File));
                            SetDirty(CurDatabase);
                        }

                        #region Remove Asset

                        if (DrawRemoveAssetButton(GUILayoutUtility.GetLastRect(), CurDatabase, CurAsset))
                            CurAsset = null;

                        #endregion Remove Asset
                    }
                    EditorGUILayout.EndHorizontal();

                    #endregion Asset Name

                    #region Info

                    if (CurAsset != null && CurAsset.AssetObject.GetType().IsSubclassOf(typeof(ScriptableObject)))
                    {
                        if (GUILayout.Button(new GUIContent("Duplicate", "Copy the object and child it to the database.")))
                        {
                            Object copy = ScriptableObject.Instantiate(CurAsset.AssetObject);
                            AssetDatabase.AddObjectToAsset(copy, CurDatabase.File);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(copy));
                            AddIconToAtlas(null, CurDatabase.AddNew(copy.name, copy));
                        }

                        if (!AssetDatabase.IsSubAsset(CurAsset.AssetObject))
                        {
                            if (CurAsset.AssetObject.GetType().IsSubclassOf(typeof(ScriptableObject)) &&
                                GUILayout.Button(new GUIContent("Import", "Child the object to the database.")))
                            {
                                Object copy = ScriptableObject.Instantiate(CurAsset.AssetObject);
                                CurAsset.Name = copy.name;
                                AssetDatabase.AddObjectToAsset(copy, CurDatabase.File);
                                CurAsset.SetObject(copy);
                                AssetDatabase.SaveAssets();
                                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(copy));
                            }
                        }
                        else if (GUILayout.Button(new GUIContent("Export", "Export a copy of the object.")))
                        {
                            Object asset = AssetTools.DuplicateAssetPanel(CurAsset.AssetObject, CurAsset.Name, "asset", out string savePath);
                            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset));
                        }
                    }

                    #endregion Info

                    GUITools.DrawHeader("Data");
                    pAssetScrollPosition = GUILayout.BeginScrollView(pAssetScrollPosition);
                    {
                        GUITools.BeginContents();
                        {
                            EditorGUILayout.LabelField("", "Asset ID: " + CurAsset.AssetKey16 + "", "HelpBox");
                            GUITools.DrawHorizontalSeparator(2, Color.gray);
                            DrawAssetData();
                        }
                        GUITools.EndContents();
                    }
                    GUILayout.EndScrollView();
                }
                else if (CurDatabase != null && !CurDatabase.DatabaseData.IsEmpty)
                    EditorGUILayout.HelpBox("You can view more Asset Options by selecting a Asset in the section above.",
                        MessageType.Info);
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws the selected assets data.
        /// </summary>
        private void DrawAssetData()
        {
            if (CurAsset != null)
            {
                if (mSerializedObject == null || mSerializedObject.targetObject == null)
                {
                    if (CurAsset != null && CurAsset.AssetObject != null)
                        mSerializedObject = new SerializedObject(CurAsset.AssetObject);
                }
                else if (CurAsset != null && mSerializedObject.targetObject != CurAsset.AssetObject)
                    mSerializedObject = new SerializedObject(CurAsset.AssetObject);

                if (CurAsset == null || mSerializedObject == null)
                    return;

                if (mSerializedObject != null)
                    DrawSerializedObject(mSerializedObject);
            }
        }

        /// <summary>
        /// Override to draw custom data for a serialized object.
        /// </summary>
        /// <param name="sObject">Serialized obejct to draw data for.</param>
        protected abstract void DrawSerializedObject(SerializedObject sObject);

        /// <summary>
        /// Removes an icon from the texture atlas of the selected database.
        /// </summary>
        /// <param name="id16">16 bit asset ID.</param>
        /// <param name="atlas">The Icon Atlas to remove the icon from.</param>
        /// <param name="parent">The database file the atlas is linked to.</param>
        protected void RemoveIconFromAtlas(short id16, IconAtlas atlas, ScriptableObject parent)
        {
            IconAtlasEditor.RemoveIconFromAtlas(id16, atlas, parent);
        }

        /// <summary>
        /// Processes dragging of assets into the database.
        /// </summary>
        public virtual void ProcessDragAssets()
        {
            bool displayError = false;

            if (mDragObjects != null)
            {
                if (CurDatabase != null)
                {
                    if (CurDatabase.File != null)
                    {
                        Object[] objs = new Object[mDragObjects.Length];
                        List<Texture2D> texs = new List<Texture2D>(mDragObjects.Length);

                        for (int i = 0; i < objs.Length; i++)
                            objs[i] = mDragObjects[i];

                        int[] keys = new int[objs.Length];
                        int index = 0;
                        foreach (Object obj in objs)
                        {
                            if (CurDatabase.DatabaseData.ContainsInstanceId(obj.GetInstanceID()))
                            {
                                Debug.Log("Database already contains asset.");
                                continue;
                            }

                            keys[index] = CurDatabase.AddNew(obj.name, obj);
                            index++;

                            texs.Add(null);
                        }

                        AddIconsToAtlas(texs.ToArray(), keys);
                        SetDirty(CurDatabase);
                    }
                    else
                        displayError = true;
                }
                else
                    displayError = true;

                if (displayError)
                {
                    EditorUtility.DisplayDialog("No Database Selected",
                        "You must first select or create a 'Database' to add the dragged Asset(s) to.",
                        "OK");
                }

                mDragObjects = null;
            }
        }
        
        /// <summary>
        /// Adds a texture to the icon database.
        /// </summary>
        /// <param name="icon">Icon texture to add.</param>
        /// <param name="key16">16 bit asset ID.</param>
        protected void AddIconToAtlas(Texture2D icon, int key16)
        {
            IconAtlasEditor.AddIconToAtlas(icon, key16, CurDatabase.IconAtlas, CurDatabase.File);
        }

        /// <summary>
        /// Adds a list of textures to the icon database.
        /// </summary>
        /// <param name="icons">Icon textures to add.</param>
        /// <param name="keys">16 bit asset IDs.</param>
        protected void AddIconsToAtlas(Texture2D[] icons, int[] keys)
        {
            IconAtlasEditor.AddIconsToAtlas(icons, keys, CurDatabase.IconAtlas, CurDatabase.File);
        }

        /// <summary>
        /// Draws the selected assets icon in a preview window.
        /// </summary>
        /// <param name="r">The icons size</param>
        /// <param name="background">GUI Style for background</param>
        /// <param name="database">The database file that contains the asset.</param>
        public virtual void DrawPreviewGUI(Rect r, GUIStyle background, IDatabaseFile database)
        {
            if (CurAsset == null)
            {
                GUI.DrawTexture(r, database.IconTexture, ScaleMode.ScaleToFit);
            }
            else
            {
                Vector2 mPreviewTxtrSize;
                Rect mEntryUvs;
                mPreviewTxtrSize = new Vector2(database.IconTexture.width, database.IconTexture.height);
                mEntryUvs = database.IconAtlas[CurAsset.AssetKey16];
                Rect mClipRect = GUITools.ClipTextureForAspect(r, new Vector2(mEntryUvs.width, mEntryUvs.height), mPreviewTxtrSize);
                GUI.DrawTextureWithTexCoords(mClipRect, database.IconTexture, mEntryUvs);
                GUILayout.Label(CurAsset.Name + " || ID: " + CurAsset.AssetKey16, "Button");
            }
        }

        /// <summary>
        /// Gets databases of a certain type
        /// </summary>
        /// <typeparam name="S">The type of database.</typeparam>
        /// <returns>Databases matching the given type.</returns>
        public virtual ScriptableObject[] GetDatabases<S>()
            where S : ScriptableObject, IDatabaseFile
        {
            mDatabases = DatabaseManager.GetDatabasesOfType<S>(Database.Visibility.Visible);

            if (mDatabases.Length > 0)
            {
                mDatabaseNames = new string[mDatabases.Length + 1];
                mDatabaseNames[0] = "Select a Database.";

                for (int i = 1; i <= mDatabases.Length; i++)
                {
                    if (mDatabases[i - 1] != null)
                        mDatabaseNames[i] = mDatabases[i - 1].name;
                    else
                        mDatabaseNames[i] = "NULL, Refresh Editor";
                }
            }
            else
            {
                mDatabaseNames = new string[] { "Create a Database." };
            }
            return mDatabases;
        }

        /// <summary>
        /// Opens a create asset panel to allow saving of a new database.
        /// </summary>
        /// <typeparam name="S">Database type.</typeparam>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="databaseId16">16 bit ID of the database.</param>
        /// <returns></returns>
        protected virtual ScriptableObject CreateNewDatabase<S>(string databaseName, short databaseId16)
            where S : ScriptableObject, IDatabaseFile
        {
            S database = AssetTools.CreateAssetPanel<S>(databaseName, "asset", out string path);

            if (database != null)
            {
                database.SetId16(databaseId16);
                CreateDatabaseFiles(path, database);
                CurDatabase = database;
            }

            return database;
        }

        /// <summary>
        /// Creates the icon atlas, and parents it to the database file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="database"></param>
        protected virtual void CreateDatabaseFiles(string path, IDatabaseFile database)
        {
            Texture2D iconTexture = new Texture2D(4, 4)
            {
                name = database.File.name + "_Icons"
            };

            AssetDatabase.AddObjectToAsset(iconTexture, database.File);
            database.IconTexture = iconTexture;
            DatabaseManager.SetDirty(database);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(database.File));
        }

        /// <summary>
        /// Verifies all asset data and deletes invalid entries.
        /// </summary>
        protected void VerifyAssetData()
        {
            if (CurDatabase == null)
            {
                CurAsset = null;
                return;
            }

            List<int> itemsToRemove = new List<int>();
            for (int i = 0; i < CurDatabase.DatabaseData.NumOfEntries; i++)
            {
                if (CurDatabase.DatabaseData.GetAssetAtIndex(i).AssetObject == null)
                    itemsToRemove.Add(i);
            }

            itemsToRemove.Sort();
            itemsToRemove.Reverse();

            if (itemsToRemove.Count > 0)
            {
                foreach (var index in itemsToRemove)
                {
                    CurDatabase.RemoveAtIndex(index);
                }

                SetDirty(CurDatabase);
                CurAsset = null;
            }
        }

        /// <summary>
        /// Draws the asset selection portion of the editor.
        /// </summary>
        /// <param name="databaseFile">Current database.</param>
        /// <param name="curAsset">Current Asset.</param>
        /// <param name="width">Width of the display area.</param>
        /// <param name="searchFilter">A string filter used to search for specific assets by name.</param>
        /// <returns>The Asset key if a new asset is selected, or -1 of no asset change is necessary.</returns>
        private int GUIAssetButtonList(IDatabaseFile databaseFile, int curAsset, float width, string searchFilter)
        {
            int index = 0;
            int selection = -1;

            DatabaseAsset entry;
            Database dB = databaseFile.DatabaseData;
            string searchText = searchFilter.ToLower();

            GUILayout.BeginVertical();
            {
                bool removed = false;
                while (index < dB.NumOfEntries)
                {
                    entry = dB.GetAssetAtIndex(index);
                    index++;

                    if (!string.IsNullOrEmpty(searchFilter) && !entry.Name.ToLower().Contains(searchText))
                        continue;

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(12);

                        Rect rect = GUILayoutUtility.GetLastRect();
                        GUITools.DrawObjectButton(new Rect(rect.x - 4, rect.y + 2, 18, 18), entry.AssetObject);

                        if (curAsset == entry.AssetKey16)
                            GUI.color = new Color(0.5770294f, 0.6585662f, 0.853f);
                        else
                            GUI.color = Color.white;

                        if (GUILayout.Button(entry.Name, (GUIStyle)"OL Title", GUILayout.Width(width - 20f)))
                            selection = entry.AssetKey16;
                        GUILayout.Space(18);
                        removed = DrawRemoveAssetButton(GUILayoutUtility.GetLastRect(), databaseFile, entry);
                    }
                    GUILayout.EndHorizontal();

                    if (removed)
                    {
                        selection = -2;
                        break;
                    }
                }
            }
            GUILayout.EndVertical();

            return selection;
        }

        /// <summary>
        /// Draws the delete asset button.
        /// </summary>
        /// <param name="lastRect">Pixel size of the button.</param>
        /// <param name="databaseFile">Current database.</param>
        /// <param name="asset">Current Asset.</param>
        /// <returns></returns>
        private bool DrawRemoveAssetButton(Rect lastRect, IDatabaseFile databaseFile, DatabaseAsset asset)
        {
            bool deletedAsset = false;

            lastRect.width = 18;
            lastRect.height = Mathf.Max(18, lastRect.height);

            GUI.color = Color.red;
            if (GUI.Button(lastRect, new GUIContent("X", "Removes the 'Asset' from the 'Database'."), "HelpBox"))
            {
                EditorApplication.Beep();
                if (EditorUtility.DisplayDialog("Remove Asset '" + asset.Name + "'",
                        "Are you sure you wish to remove this 'Asset' from the 'Database'?",
                        "Confirm",
                        "Cancel"))
                {
                    IconAtlasEditor.RemoveIconFromAtlas(asset.AssetKey16, databaseFile.IconAtlas, databaseFile.File);
                    databaseFile.DatabaseData.RemoveAssetByValue(asset);

                    if (AssetDatabase.IsSubAsset(asset.AssetObject))
                        ScriptableObject.DestroyImmediate(asset.AssetObject, true);

                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(databaseFile.File));
                    DatabaseManager.SetDirty(databaseFile);
                    deletedAsset = true;
                    GUI.color = Color.white;
                }
            }
            GUI.color = Color.white;

            return deletedAsset;
        }
    }
}