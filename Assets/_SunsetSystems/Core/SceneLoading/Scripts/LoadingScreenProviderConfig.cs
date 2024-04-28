using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using SunsetSystems.Utils.Extensions;
using SunsetSystems.Core.AddressableManagement;

namespace SunsetSystems.Core.SceneLoading.UI
{
    [CreateAssetMenu(fileName = "New Loading Screen Config", menuName = "Sunset Core/Loading Screen Provider Config")]
    public class LoadingScreenProviderConfig : SerializedScriptableObject, IAddressableAssetSource<Sprite>
    {
        [SerializeField]
        private List<AssetReference> defaultLoadingScreens = new();

        private readonly List<AssetReferenceSprite> loadedScreens = new();

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
            List<AssetReferenceSprite> toRelease = new(loadedScreens);
            toRelease.ForEach(screen => ReturnAsset(screen));
        }

        public async Task<Sprite> GetRandomLoadingScreenAsync()
        {
            AssetReferenceSprite loadingScreenAssetRef = defaultLoadingScreens.GetRandom() as AssetReferenceSprite;
            return await GetAssetAsync(loadingScreenAssetRef);
        }

        public void ReleaseLoadingScreens()
        {
            List<AssetReferenceSprite> toRelease = new(loadedScreens);
            toRelease.ForEach(screen => ReturnAsset(screen));
        }

        public async Task<Sprite> GetAssetAsync(AssetReferenceT<Sprite> assetReference)
        {
            loadedScreens.Add(assetReference as AssetReferenceSprite);
            return await AddressableManager.Instance.LoadAssetAsync(assetReference);
        }

        public void ReturnAsset(AssetReferenceT<Sprite> asset)
        {
            loadedScreens.Remove(asset as AssetReferenceSprite);
            AddressableManager.Instance.ReleaseAsset(asset);
        }
    }
}
