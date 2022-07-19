using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SunsetSystems.Bootstraper.Editor
{
    public class BootstraperSettingsWindow : EditorWindow
    {
        private const string START_SCENE_PATH = "START_SCENE_PATH";

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorSceneManager.playModeStartScene = (SceneAsset)EditorGUILayout.ObjectField(new GUIContent("Start Scene"), EditorSceneManager.playModeStartScene, typeof(SceneAsset), false);
            if (EditorGUI.EndChangeCheck())
            {
                HandleStartSceneCaching();
            }

        }

        [InitializeOnLoadMethod]
        private static void HandleStartSceneCaching()
        {
            if (!EditorSceneManager.playModeStartScene)
            {
                string path = EditorPrefs.GetString(START_SCENE_PATH, "");
                if (!path.Equals(""))
                {
                    EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                }
            }
            if (EditorSceneManager.playModeStartScene)
            {
                string path = AssetDatabase.GetAssetOrScenePath(EditorSceneManager.playModeStartScene);
                EditorPrefs.SetString(START_SCENE_PATH, path);
            }
        }

        [MenuItem("Boostrapper/Settings")]
        static void Open()
        {
            GetWindow<BootstraperSettingsWindow>();
        }
    }
}
