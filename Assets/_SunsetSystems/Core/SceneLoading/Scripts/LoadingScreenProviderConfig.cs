using System.Collections.Generic;
using System.Threading.Tasks;
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

        private readonly List<AssetReference> loadedScreens = new();

        //private void OnValidate()
        //{
        //    List<AssetReference> nonSprites = new();
        //    foreach (var assetRef in defaultLoadingScreens)
        //    {
        //        if (assetRef != null && assetRef is not AssetReferenceSprite)
        //            nonSprites.Add(assetRef);
        //    }
        //    defaultLoadingScreens.RemoveAll(assetRef => nonSprites.Contains(assetRef));
        //}

        private void OnEnable()
        {
            loadedScreens.Clear();
        }

        private void OnDisable()
        {
            List<AssetReference> toRelease = new(loadedScreens);
            toRelease.ForEach(screen => ReturnAsset(screen));
        }

        public async Task<Sprite> GetRandomLoadingScreenAsync()
        {
            AssetReferenceSprite loadingScreenAssetRef = defaultLoadingScreens.GetRandom() as AssetReferenceSprite;
            return await GetAssetAsync(loadingScreenAssetRef);
        }

        public void ReleaseLoadingScreens()
        {
            List<AssetReference> toRelease = new(loadedScreens);
            toRelease.ForEach(screen => ReturnAsset(screen));
        }

        public async Task<Sprite> GetAssetAsync(AssetReference assetReference)
        {
            loadedScreens.Add(assetReference);
            return await AddressableManager.Instance.LoadAssetAsync<Sprite>(assetReference);
        }

        public void ReturnAsset(AssetReference asset)
        {
            loadedScreens.Remove(asset);
            AddressableManager.Instance.ReleaseAsset(asset);
        }
    }
}
