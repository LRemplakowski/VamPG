using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SunsetSystems.Bootstraper.Editor
{
    public class BootstraperSettingsWindow : EditorWindow
    {
        private const string START_SCENE_PATH = "START_SCENE_PATH";
        private static SceneAsset cachedStartScene;
        private static bool enableBootstrap;

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            cachedStartScene = (SceneAsset)EditorGUILayout.ObjectField(new GUIContent("Start Scene"), cachedStartScene, typeof(SceneAsset), false);
            enableBootstrap = EditorGUILayout.Toggle(new GUIContent("Enable Bootstrapping"), enableBootstrap);
            if (EditorGUI.EndChangeCheck())
            {
                HandleStartSceneCaching();
            }
        }

        [InitializeOnLoadMethod]
        private static void HandleStartSceneCaching()
        {
            if (!cachedStartScene)
            {
                string path = EditorPrefs.GetString(START_SCENE_PATH, "");
                if (!path.Equals(""))
                {
                    cachedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                }
            }
            if (cachedStartScene)
            {
                string path = AssetDatabase.GetAssetOrScenePath(cachedStartScene);
                EditorPrefs.SetString(START_SCENE_PATH, path);
            }
            EditorSceneManager.playModeStartScene = enableBootstrap ? cachedStartScene : null;
        }

        [MenuItem("Boostrapper/Settings")]
        static void Open()
        {
            GetWindow<BootstraperSettingsWindow>();
        }
    }
}
