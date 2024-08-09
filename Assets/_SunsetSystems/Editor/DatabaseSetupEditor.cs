using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using SunsetSystems.Core.Database;
using SunsetSystems.Journal;
using UnityEditor;
using UnityEngine;

namespace SunsetSystems.Editor
{
    [InitializeOnLoad]
    public class DatabaseSetupEditor : OdinEditorWindow
    {
        static DatabaseSetupEditor()
        {
            GetWindow<DatabaseSetupEditor>().Close();
        }

        [AssetsOnly]
        public ItemDatabase ItemDatabase;
        [AssetsOnly]
        public CreatureDatabase CreatureDatabase;
        [AssetsOnly]
        public QuestDatabase QuestDatabase;
        [AssetsOnly]
        public ObjectiveDatabase ObjectiveDatabase;

        [MenuItem("Tools/Database Setup")]
        public static void OpenWindow()
        {
            var window = GetWindow<DatabaseSetupEditor>();
            window.OnClose += window.SaveChanges;
            window.Show();
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
            SaveDatabaseReferences();
            UpdateStaticReferences();
        }

        protected override void Initialize()
        {
            base.Initialize();
            LoadDatabaseReferences();
            UpdateStaticReferences();
        }

        private void LoadDatabaseReferences()
        {
            string itemPath = EditorPrefs.GetString("ItemDatabase");
            ItemDatabase = AssetDatabase.LoadAssetAtPath<ItemDatabase>(itemPath);
            string creaturePath = EditorPrefs.GetString("CreatureDatabase");
            CreatureDatabase = AssetDatabase.LoadAssetAtPath<CreatureDatabase>(creaturePath);
            string questPath = EditorPrefs.GetString("QuestDatabase");
            QuestDatabase = AssetDatabase.LoadAssetAtPath<QuestDatabase>(questPath);
            string objectivePath = EditorPrefs.GetString("ObjectiveDatabase");
            ObjectiveDatabase = AssetDatabase.LoadAssetAtPath<ObjectiveDatabase>(objectivePath);
        }

        private void SaveDatabaseReferences()
        {
            string itemPath = AssetDatabase.GetAssetPath(ItemDatabase);
            EditorPrefs.SetString("ItemDatabase", itemPath);
            string creatureParth = AssetDatabase.GetAssetPath(CreatureDatabase);
            EditorPrefs.SetString("CreatureDatabase", creatureParth);
            string questPath = AssetDatabase.GetAssetPath(QuestDatabase);
            EditorPrefs.SetString("QuestDatabase", questPath);
            string objectivePath = AssetDatabase.GetAssetPath(ObjectiveDatabase);
            EditorPrefs.SetString("ObjectiveDatabase", objectivePath);
        }

        private void UpdateStaticReferences()
        {
            Debug.Log("Editor Database references updated!");
            EditorDatabaseHelper.ItemDB = ItemDatabase;
            EditorDatabaseHelper.CreatureDB = CreatureDatabase;
            EditorDatabaseHelper.QuestDB = QuestDatabase;
            EditorDatabaseHelper.ObjectiveDB = ObjectiveDatabase;
        }
    }
}
