using System.Collections.Generic;
using System.Threading.Tasks;
using Redcode.Awaiting;
using Sirenix.OdinInspector;
using SunsetSystems.Core.AddressableManagement;
using SunsetSystems.Utils.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Core.SceneLoading.UI
{
    [CreateAssetMenu(fileName = "New Loading Screen Config", menuName = "Sunset Core/Loading Screen Provider Config")]
    public class LoadingScreenProviderConfig : SerializedScriptableObject, IAddressableAssetSource<Sprite>
    {
        [SerializeField]
        private List<AssetReference> defaultLoadingScreens = new();
        [ShowInInspector, ReadOnly]
        private readonly List<AssetReference> _loadedScreens = new();

        private void Awake()
        {
            _loadedScreens.Clear();
        }

        public async Task<Sprite> GetRandomLoadingScreenAsync()
        {
            AssetReference loadingScreenAssetRef = defaultLoadingScreens.GetRandom();
            return await GetAssetAsync(loadingScreenAssetRef);
        }

        public void ReleaseLoadingScreens()
        {
            List<AssetReference> toRelease = new(_loadedScreens);
            toRelease.ForEach(screen => ReturnAsset(screen));
        }

        public async Task<Sprite> GetAssetAsync(AssetReference assetReference)
        {
            _loadedScreens.Add(assetReference);
            var asyncOp = Addressables.LoadAssetAsync<Sprite>(assetReference);
            await asyncOp.Task;
            return asyncOp.Result;
        }

        public void ReturnAsset(AssetReference asset)
        {
            if (_loadedScreens.Remove(asset))
                Addressables.Release(asset);
        }
    }
}
