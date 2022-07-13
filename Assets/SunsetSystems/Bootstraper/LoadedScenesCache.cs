using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace SunsetSystems.Bootstraper
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    internal static class LoadedScenesCache
    {
        private const int GAME_SCENE_INDEX = 0;
        private const int UI_SCENE_INDEX = 1;

        private static readonly List<Scene> _cachedScenes = new();
        public static IReadOnlyCollection<Scene> CachedScenes => _cachedScenes;
        static LoadedScenesCache()
        {
            EditorApplication.playModeStateChanged += OnEnterPlayMode;
        }

        private static void OnEnterPlayMode(PlayModeStateChange state)
        {
            if (state.Equals(PlayModeStateChange.ExitingEditMode))
            {
                _cachedScenes.Clear();

                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    if (scene.buildIndex != GAME_SCENE_INDEX && scene.buildIndex != UI_SCENE_INDEX)
                        _cachedScenes.Add(scene);
                }
            }
        }
    }
#endif
}
