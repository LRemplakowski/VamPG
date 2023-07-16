using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityQuickSheet;

namespace PixelCrushers.DialogueSystem.Articy
{

    /// <summary>
    /// This is a supplemental tool for articy:draft 3+'s localization plugin. It reads the 
    /// plugin's Excel files and applies the localization content to a dialogue database that 
    /// you've previously created with the regular articy:draft Converter.
    /// 
    /// NOTE: Requires https://github.com/kimsama/Unity-QuickSheet
    /// </summary>
    public class ArticyLocalizationImporter : EditorWindow
    {

        [MenuItem("Tools/Pixel Crushers/Dialogue System/Import/articy:draft Localization Importer", false, 99)]
        public static void Init()
        {
            GetWindow(typeof(ArticyLocalizationImporter), false, "Localization");
        }

        private const string MainFilepathKey = "PixelCrushers.Articy.Localization.File";

        private string mainFilepath { get; set; }

        private string xmlFilepath { get; set; }

        private DialogueDatabase database { get; set; }

        private string currentFilename { get; set; }

        private bool debug { get; set; }

        private ConverterPrefs prefs = new ConverterPrefs();

        private Dictionary<string, Conversation> conversationsByTechnicalName = new Dictionary<string, Conversation>();

        private Dictionary<string, DialogueEntry> dialogueEntriesByTechnicalName = new Dictionary<string, DialogueEntry>();

        private void OnEnable()
        {
            mainFilepath = EditorPrefs.GetString(MainFilepathKey);
            prefs = ConverterPrefsTools.Load();
            xmlFilepath = prefs.ProjectFilename;
        }

        private void OnDisable()
        {
            EditorPrefs.SetString(MainFilepathKey, mainFilepath);
        }

        private void OnGUI()
        {
            DrawXmlFilepath();
            DrawMainExcelFilepath();
            DrawDatabase();
            DrawButtons();
        }

        private void DrawXmlFilepath()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(new GUIContent("articy:draft Project", "The XML file that you exported from articy:draft. Remember to UNtick Export Text Markup when exporting."), xmlFilepath);
            if (GUILayout.Button("...", EditorStyles.miniButtonRight, GUILayout.Width(22)))
            {
                xmlFilepath = EditorUtility.OpenFilePanel("Select articy:draft Project", EditorWindowTools.GetDirectoryName(xmlFilepath), "xml");
                GUIUtility.keyboardControl = 0;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawMainExcelFilepath()
        {
            EditorGUILayout.BeginHorizontal();
            mainFilepath = EditorGUILayout.TextField(new GUIContent("Main Excel File", "Main Excel file created by the articy:draft localization plugin."), mainFilepath);
            if (GUILayout.Button("...", EditorStyles.miniButtonRight, GUILayout.Width(22)))
            {
                mainFilepath =
                    EditorUtility.OpenFilePanel("Select Main Excel File",
                                                EditorWindowTools.GetDirectoryName(mainFilepath),
                                                "xlsx");
                GUIUtility.keyboardControl = 0;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawDatabase()
        {
            database = EditorGUILayout.ObjectField(new GUIContent("Dialogue Database", "Dialogue database to update with content from localization Excel files."), database, typeof(DialogueDatabase), false) as DialogueDatabase;
        }

        private void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal();
            debug = EditorGUILayout.Toggle(new GUIContent("Debug", "Log any issues to the Console."), debug);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Help", GUILayout.Width(96))) Application.OpenURL("https://www.pixelcrushers.com/dialogue_system/manual2x/html/articy_draft.html#articyLocalization");
            EditorGUI.BeginDisabledGroup(database == null || string.IsNullOrEmpty(xmlFilepath) || string.IsNullOrEmpty(mainFilepath));
            if (GUILayout.Button("Import Excel Files", GUILayout.Width(128))) ImportExcelFiles();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal(); ;
        }

        private void ImportExcelFiles()
        {
            try
            {
                EditorUtility.DisplayCancelableProgressBar("articy:draft Localization Importer", "Reading XML file", 0);
                if (CreateTechnicalNameIndexCancelable()) return;
                EditorUtility.DisplayCancelableProgressBar("articy:draft Localization Importer", "Importing Excel files", 0);
                var path = Path.GetDirectoryName(mainFilepath);
                var mainFilename = Path.GetFileName(mainFilepath);
                var mainFilenameNoExtension = mainFilename.Substring(0, mainFilename.Length - "en.xlsx".Length);
                var wildcard = mainFilenameNoExtension + "*.xlsx";
                var filepaths = Directory.GetFiles(path, wildcard);
                for (int i = 0; i < filepaths.Length; i++)
                {
                    var filepath = filepaths[i];
                    var filename = Path.GetFileName(filepath);
                    currentFilename = filename;
                    if (EditorUtility.DisplayCancelableProgressBar("articy:draft Localization Importer", "Importing " + filename, (float)i / (float)filepaths.Length)) break;
                    var languageCode = Path.GetFileNameWithoutExtension(filepath).Substring(mainFilenameNoExtension.Length);
                    try
                    {
                        ProcessExcelFile(languageCode, filepath);
                    }
                    catch (System.ArgumentException e)
                    {
                        Debug.LogException(e);
                        EditorUtility.DisplayDialog("Error Reading Excel File", "Unable to read " + filename +
                            ". This importer has trouble reading articy-generated XLSX files. Please open this file in a spreadsheet program such as MS Excel or LibreOffice and save it. The importer should then be able to read it.", "OK");
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private bool CreateTechnicalNameIndexCancelable()
        {
            conversationsByTechnicalName.Clear();
            dialogueEntriesByTechnicalName.Clear();

            // Load XML file:
            var articyData = ArticySchemaEditorTools.LoadArticyDataFromXmlFile(xmlFilepath, prefs.Encoding, prefs.ConvertDropdownsAs, prefs);
            if (articyData == null) return true;

            // Index conversations:
            foreach (var dialogue in articyData.dialogues.Values)
            {
                if (EditorUtility.DisplayCancelableProgressBar("articy:draft Localization Importer", dialogue.technicalName, 0)) return true;
                var conversation = database.conversations.Find(x => string.Equals(x.LookupValue("Articy Id"), dialogue.id));
                if (conversation == null)
                {
                    if (debug) Debug.LogWarning("Dialogue System: Dialogue database doesn't have a conversation with Articy Id = " + dialogue.id, database);
                }
                else
                {
                    conversationsByTechnicalName[dialogue.technicalName] = conversation;
                }
            }

            // Index dialogue entries:
            foreach (var dialogueFragment in articyData.dialogueFragments.Values)
            {
                if (IndexDialogueEntryTechnicalNameCancelable(dialogueFragment.id, dialogueFragment.technicalName)) return true;
            }
            foreach (var hub in articyData.hubs.Values)
            {
                if (IndexDialogueEntryTechnicalNameCancelable(hub.id, hub.technicalName)) return true;
            }
            foreach (var jump in articyData.jumps.Values)
            {
                if (IndexDialogueEntryTechnicalNameCancelable(jump.id, jump.technicalName)) return true;
            }
            foreach (var flowFragment in articyData.flowFragments.Values)
            {
                if (IndexDialogueEntryTechnicalNameCancelable(flowFragment.id, flowFragment.technicalName)) return true;
            }
            return false;
        }

        private bool IndexDialogueEntryTechnicalNameCancelable(string articyId, string technicalName)
        {
            if (EditorUtility.DisplayCancelableProgressBar("articy:draft Localization Importer", technicalName, 0)) return true;
            for (int i = 0; i < database.conversations.Count; i++)
            {
                var conversation = database.conversations[i];
                var entry = conversation.dialogueEntries.Find(x => string.Equals(Field.LookupValue(x.fields, "Articy Id"), articyId));
                if (entry != null)
                {
                    if (dialogueEntriesByTechnicalName.ContainsKey(technicalName))
                    {
                        Debug.LogWarning("Technical Name " + technicalName + " is used by two or more dialogue entries, including '" + entry.DialogueText + "'.");
                    }
                    else
                    {
                        dialogueEntriesByTechnicalName.Add(technicalName, entry);
                    }
                }
            }
            return false;
        }

        private void ProcessExcelFile(string languageCode, string filename)
        {
            ExcelQuery query = new ExcelQuery(filename, "ArticyStrings");
            if (query == null || !query.IsValid())
            {
                Debug.LogError("Can't load " + filename + "!");
                return;
            }

            var rows = query.Deserialize<ArticyLocaData>().ToArray();

            for (int i = 0; i < rows.Length; i++)
            {
                var row = rows[i];
                var locaID = row.LocaID;
                var value = row.Value;
                var contextPath = row.ContextPath;
                var dotPos = locaID.IndexOf('.');
                var technicalName = locaID.Substring(0, dotPos);
                var fieldSpec = locaID.Substring(dotPos + 1);
                if (contextPath.StartsWith("Assets")) continue;
                SetLocalizedField(languageCode, technicalName, fieldSpec, value);
            }
        }

        void SetLocalizedField(string languageCode, string technicalName, string fieldSpec, string value)
        {
            if (technicalName.StartsWith("Ntt") || technicalName.StartsWith("Chr") || technicalName.StartsWith("Itm")) //[UPDATED]
            {
                // Entity:
                var actor = (technicalName.StartsWith("Ntt") || technicalName.StartsWith("Chr"))
                    ? database.actors.Find(x => string.Equals(x.LookupValue("Technical Name"), technicalName))
                    : null;
                if (actor != null)
                {
                    SetLocalizedFieldInFieldsList(actor.fields, languageCode, fieldSpec, value, false);
                }
                else if (!technicalName.StartsWith("Chr"))
                {
                    var item = database.items.Find(x => string.Equals(x.LookupValue("Technical Name"), technicalName));
                    if (item != null)
                    {
                        SetLocalizedFieldInFieldsList(item.fields, languageCode, fieldSpec, value, false);
                    }
                    else
                    {
                        if (debug) Debug.LogWarning("Dialogue System: " + currentFilename + ": Not localizing " + technicalName + "." + fieldSpec + ". Can't find in actors, items, or quests.");
                    }
                }
            }
            else if (technicalName.StartsWith("FFr"))
            {
                // Flow Fragment:
                var item = database.items.Find(x => string.Equals(x.LookupValue("Technical Name"), technicalName));
                if (item != null)
                {
                    SetLocalizedFieldInFieldsList(item.fields, languageCode, fieldSpec, value, false);
                }
                else
                {
                    var entry = FindDialogueEntryByTechnicalName(technicalName);
                    if (entry != null)
                    {
                        SetLocalizedFieldInFieldsList(entry.fields, languageCode, fieldSpec, value, true);
                    }
                    else
                    {
                        if (debug) Debug.LogWarning("Dialogue System: " + currentFilename + ": Not localizing " + technicalName + "." + fieldSpec + ". Can't find in conversations or quests. (You can usually ignore this if the flow fragment isn't a quest or part of a dialogue.)");
                    }
                }
            }
            else if (technicalName.StartsWith("DFr") || technicalName.StartsWith("Hub") || technicalName.StartsWith("Jmp"))
            {
                // Dialogue Fragment, Hub, or Jump:
                var entry = FindDialogueEntryByTechnicalName(technicalName);
                if (entry != null)
                {
                    SetLocalizedFieldInFieldsList(entry.fields, languageCode, fieldSpec, value, true);
                }
                else
                {
                    if (debug)
                    {
                        if (technicalName.StartsWith("Jmp"))
                        {
                            Debug.LogWarning("Dialogue System: " + currentFilename + ": Not localizing " + technicalName + "." + fieldSpec + ". Can't find any matching dialogue entry. (Some jumps are converted differently into the Dialogue System and localization might be ambiguous.)");
                        }
                        else
                        {
                            Debug.LogWarning("Dialogue System: " + currentFilename + ": Not localizing " + technicalName + "." + fieldSpec + ". Can't find any matching dialogue entry.");
                        }
                    }
                }
            }
            else if (technicalName.StartsWith("Dlg"))
            {
                // Dialogue:
                var conversation = FindConversationsByTechnicalName(technicalName);
                if (conversation != null)
                {
                    SetLocalizedFieldInFieldsList(conversation.fields, languageCode, fieldSpec, value, false);
                }
                else
                {
                    if (debug) Debug.LogWarning("Dialogue System: " + currentFilename + ": Not localizing " + technicalName + "." + fieldSpec + ". Can't find matching conversation.");
                }
            }
            else if (technicalName.StartsWith("Cnd"))
            {
                // No need to localize conditions.
            }
            else
            {
                // Find asset in database by technical name:
                var actor = database.actors.Find(x => x.LookupValue("Technical Name") == technicalName);
                if (actor != null)
                {
                    SetLocalizedFieldInFieldsList(actor.fields, languageCode, fieldSpec, value, false);
                }
                else
                {
                    var item = database.items.Find(x => x.LookupValue("Technical Name") == technicalName);
                    if (item != null)
                    {
                        SetLocalizedFieldInFieldsList(item.fields, languageCode, fieldSpec, value, false);
                    }
                    else
                    {
                        var entry = FindDialogueEntryByTechnicalName(technicalName);
                        if (entry != null)
                        {
                            SetLocalizedFieldInFieldsList(entry.fields, languageCode, fieldSpec, value, true);
                        }
                        else
                        {
                            var conversation = FindConversationsByTechnicalName(technicalName);
                            if (conversation != null)
                            {
                                SetLocalizedFieldInFieldsList(conversation.fields, languageCode, fieldSpec, value, false);
                            }
                            else if (!fieldSpec.EndsWith(".DisplayName"))
                            {
                                if (debug) Debug.LogWarning("Dialogue System: " + currentFilename + ": Don't know how to localize " + technicalName + "." + fieldSpec + ".");
                            }
                        }
                    }
                }
            }
        }

        Conversation FindConversationsByTechnicalName(string technicalName)
        {
            return conversationsByTechnicalName.ContainsKey(technicalName) ? conversationsByTechnicalName[technicalName] : null;
        }

        DialogueEntry FindDialogueEntryByTechnicalName(string technicalName)
        {
            return dialogueEntriesByTechnicalName.ContainsKey(technicalName) ? dialogueEntriesByTechnicalName[technicalName] : null;
        }

        void SetLocalizedFieldInFieldsList(List<Field> fields, string languageCode, string fieldSpec, string value, bool isDialogueEntry)
        {
            if (fields == null || string.IsNullOrEmpty(fieldSpec)) return;
            switch (fieldSpec)
            {
                case "DisplayName":
                    if (Field.FieldExists(fields, "Display Name"))
                    {
                        SetFieldInFieldsList(fields, "Display Name " + languageCode, value);
                    }
                    else
                    {
                        SetFieldInFieldsList(fields, "Name " + languageCode, value);
                    }
                    break;
                case "Text":
                    SetFieldInFieldsList(fields, (isDialogueEntry ? string.Empty : "Description ") + languageCode, value);
                    break;
                case "PreviewText":
                    if (isDialogueEntry) SetFieldInFieldsList(fields, "Menu Text " + languageCode, value);
                    break;
                case "StageDirections":
                    if (prefs.StageDirectionsMode == ConverterPrefs.StageDirModes.Sequences) SetFieldInFieldsList(fields, "Sequence " + languageCode, value);
                    break;
                default:
                    SetFieldInFieldsList(fields, fieldSpec + " " + languageCode, value);
                    break;
            }
        }

        void SetFieldInFieldsList(List<Field> fields, string title, string value)
        {
            if (fields == null || string.IsNullOrEmpty(title)) return;
            var field = Field.Lookup(fields, title);
            if (field != null)
            {
                field.value = value;
            }
            else
            {
                fields.Add(new Field(title, value, FieldType.Localization));
            }
        }

    }
}
