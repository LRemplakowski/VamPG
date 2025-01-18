using SunsetSystems.Core.Database;
using SunsetSystems.Journal;
using SunsetSystems.WorldMap;
using UnityEditor;

namespace SunsetSystems.Editor
{
    public class DatabaseAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            string itemPath = EditorPrefs.GetString("ItemDatabase");
            EditorDatabaseHelper.ItemDB = AssetDatabase.LoadAssetAtPath<ItemDatabase>(itemPath);
            string creaturePath = EditorPrefs.GetString("CreatureDatabase");
            EditorDatabaseHelper.CreatureDB = AssetDatabase.LoadAssetAtPath<CreatureDatabase>(creaturePath);
            string questPath = EditorPrefs.GetString("QuestDatabase");
            EditorDatabaseHelper.QuestDB = AssetDatabase.LoadAssetAtPath<QuestDatabase>(questPath);
            string objectivePath = EditorPrefs.GetString("ObjectiveDatabase");
            EditorDatabaseHelper.ObjectiveDB = AssetDatabase.LoadAssetAtPath<ObjectiveDatabase>(objectivePath);
            string wardrobePath = EditorPrefs.GetString("WardrobeDatabase");
            EditorDatabaseHelper.WardrobeDB = AssetDatabase.LoadAssetAtPath<UMAWardrobeDatabase>(wardrobePath);
            string worldMapPath = EditorPrefs.GetString("WorldMapDatabase");
            EditorDatabaseHelper.WorldMapDB = AssetDatabase.LoadAssetAtPath<WorldMapEntryDatabase>(worldMapPath);
        }

        [InitializeOnEnterPlayMode]
        private static void OnEnterPlayMode()
        {
            string itemPath = EditorPrefs.GetString("ItemDatabase");
            EditorDatabaseHelper.ItemDB = AssetDatabase.LoadAssetAtPath<ItemDatabase>(itemPath);
            string creaturePath = EditorPrefs.GetString("CreatureDatabase");
            EditorDatabaseHelper.CreatureDB = AssetDatabase.LoadAssetAtPath<CreatureDatabase>(creaturePath);
            string questPath = EditorPrefs.GetString("QuestDatabase");
            EditorDatabaseHelper.QuestDB = AssetDatabase.LoadAssetAtPath<QuestDatabase>(questPath);
            string objectivePath = EditorPrefs.GetString("ObjectiveDatabase");
            EditorDatabaseHelper.ObjectiveDB = AssetDatabase.LoadAssetAtPath<ObjectiveDatabase>(objectivePath);
            string wardrobePath = EditorPrefs.GetString("WardrobeDatabase");
            EditorDatabaseHelper.WardrobeDB = AssetDatabase.LoadAssetAtPath<UMAWardrobeDatabase>(wardrobePath);
            string worldMapPath = EditorPrefs.GetString("WorldMapDatabase");
            EditorDatabaseHelper.WorldMapDB = AssetDatabase.LoadAssetAtPath<WorldMapEntryDatabase>(worldMapPath);
        }
    }
}
