using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace SunsetSystems.Bootstraper
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    internal static class LoadedScenesCache
    {
        private const int GAME_SCENE_INDEX = 0;
        private const int UI_SCENE_INDEX = 1;

        private static readonly List<string> _cachedScenes = new();
        public static List<string> CachedScenes => new(_cachedScenes);
        static LoadedScenesCache()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneClosed += OnSceneClosed;
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            _cachedScenes.Clear();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene sceneAtIndex = SceneManager.GetSceneAt(i);
                if (sceneAtIndex.buildIndex != GAME_SCENE_INDEX && sceneAtIndex.buildIndex != UI_SCENE_INDEX)
                    _cachedScenes.Add(sceneAtIndex.path);
            }
        }

        private static void OnSceneClosed(Scene scene)
        {
            _cachedScenes.Remove(scene.path);
        }
    }
#endif
}
