using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System;
using SunsetSystems.Audio;
using Sirenix.Serialization;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Core.SceneLoading
{
    [CreateAssetMenu(fileName = "loadingData_NewSceneLoadingData", menuName = "Scene Loading Data")]
    public class SceneLoadingDataAsset : SerializedScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField, ES3NonSerializable]
        private List<UnityEditor.SceneAsset> sceneAssets = new();

        private void OnValidate()
        {
            _loadingData.ScenePaths = new();
            foreach (UnityEditor.SceneAsset asset in sceneAssets)
            {
                LoadingData.ScenePaths.Add(UnityEditor.AssetDatabase.GetAssetPath(asset));
            }
        }
#endif
        [SerializeField]
        private LevelLoadingData _loadingData = new();
        public LevelLoadingData LoadingData => _loadingData;
    }

    [Serializable]
    public struct LevelLoadingData
    {
        [SerializeField]
        public bool ShowScenePaths;
        [SerializeField, ReadOnly, ShowIf("ShowScenePaths")]
        public List<string> ScenePaths;
        public readonly IReadOnlyList<string> AddressableScenePaths => ScenePaths.AsReadOnly();
    }
}
