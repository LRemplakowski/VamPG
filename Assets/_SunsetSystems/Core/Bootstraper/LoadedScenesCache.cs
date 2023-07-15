#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace SunsetSystems.Bootstraper
{
    [InitializeOnLoad]
    internal static class LoadedScenesCache
    {
        private const int GAME_SCENE_INDEX = 0;
        private const int UI_SCENE_INDEX = 1;

        private static readonly List<string> _cachedScenes = new();
        public static List<string> CachedScenes => new(_cachedScenes);
        private static bool rebuildCache = true;
        static LoadedScenesCache()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorApplication.playModeStateChanged += OnEditorStateChange;
        }

        private static void OnEditorStateChange(PlayModeStateChange mode)
        {
            if (mode.Equals(PlayModeStateChange.ExitingEditMode))
            {
                rebuildCache = false;
            }
            if (mode.Equals(PlayModeStateChange.EnteredEditMode))
            {
                rebuildCache = true;
            }
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (!rebuildCache)
                return;
            _cachedScenes.Clear();
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                Scene sceneAtIndex = EditorSceneManager.GetSceneAt(i);
                if (sceneAtIndex.buildIndex != GAME_SCENE_INDEX && sceneAtIndex.buildIndex != UI_SCENE_INDEX && !_cachedScenes.Contains(sceneAtIndex.path))
                    _cachedScenes.Add(sceneAtIndex.path);
            }
        }
    }
}
#endif
