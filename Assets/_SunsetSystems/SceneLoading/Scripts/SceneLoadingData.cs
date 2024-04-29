using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SunsetSystems.Core.SceneLoading
{
    [CreateAssetMenu(fileName = "loadingData_NewSceneLoadingData", menuName = "Scene Loading Data")]
    public class SceneLoadingData : SerializedScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField]
        private List<SceneAsset> sceneAssets = new();

        private void OnValidate()
        {
            scenePaths = new();
            foreach (SceneAsset asset in sceneAssets)
            {
                scenePaths.Add(AssetDatabase.GetAssetPath(asset));
            }
        }
#endif
        [SerializeField]
        private bool showScenePaths = false;
        [SerializeField, ReadOnly, ShowIf("showScenePaths")]
        private List<string> scenePaths = new();
        public IReadOnlyList<string> AddressableScenePaths => scenePaths.AsReadOnly();
    }
}
