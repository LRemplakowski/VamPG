using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SunsetSystems.Bootstraper.Editor
{
    public class BootstraperSettingsWindow : EditorWindow
    {
        private void OnGUI()
        {
            EditorSceneManager.playModeStartScene = (SceneAsset)EditorGUILayout.ObjectField(new GUIContent("Start Scene"), EditorSceneManager.playModeStartScene, typeof(SceneAsset), false);
        }

        [MenuItem("Boostrapper/Settings")]
        static void Open()
        {
            GetWindow<BootstraperSettingsWindow>();
        }
    }
}
