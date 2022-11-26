using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SunsetSystems.Bootstraper.Editor
{
    public class BootstraperSettingsWindow : EditorWindow
    {
        private const string START_SCENE_PATH = "START_SCENE_PATH";
        private const string ENABLE_BOOTSTRAP_CACHE = "ENABLE_BOOTSTRAP";
        private static SceneAsset cachedStartScene;

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            cachedStartScene = (SceneAsset)EditorGUILayout.ObjectField(new GUIContent("Start Scene"), cachedStartScene, typeof(SceneAsset), false);
            SunsetBootstraper.EnableBootstrap = EditorGUILayout.Toggle(new GUIContent("Enable Bootstrapping"), SunsetBootstraper.EnableBootstrap);
            if (EditorGUI.EndChangeCheck())
            {
                HandleStartSceneCaching();
            }
        }

        [InitializeOnLoadMethod]
        private static void LoadFromCache()
        {
            if (!cachedStartScene)
            {
                string path = EditorPrefs.GetString(START_SCENE_PATH, "");
                if (!path.Equals(""))
                {
                    cachedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                }
            }
            SunsetBootstraper.EnableBootstrap = EditorPrefs.GetBool(ENABLE_BOOTSTRAP_CACHE, false);
            EditorSceneManager.playModeStartScene = SunsetBootstraper.EnableBootstrap ? cachedStartScene : null;
        }

        private static void HandleStartSceneCaching()
        {
            if (cachedStartScene)
            {
                string path = AssetDatabase.GetAssetOrScenePath(cachedStartScene);
                EditorPrefs.SetString(START_SCENE_PATH, path);
            }
            EditorPrefs.SetBool(ENABLE_BOOTSTRAP_CACHE, SunsetBootstraper.EnableBootstrap);
            EditorSceneManager.playModeStartScene = SunsetBootstraper.EnableBootstrap ? cachedStartScene : null;
        }

        [MenuItem("Boostrapper/Settings")]
        static void Open()
        {
            GetWindow<BootstraperSettingsWindow>();
        }
    }
}
