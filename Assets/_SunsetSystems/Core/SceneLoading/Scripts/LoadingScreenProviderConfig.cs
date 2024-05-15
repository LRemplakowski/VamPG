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

        //private void OnEnable()
        //{
        //    loadedScreens.Clear();
        //}

        //private void OnDisable()
        //{
        //    List<AssetReference> toRelease = new(loadedScreens);
        //    toRelease.ForEach(screen => ReturnAsset(screen));
        //}

        public async Task<Sprite> GetRandomLoadingScreenAsync()
        {
            AssetReference loadingScreenAssetRef = defaultLoadingScreens.GetRandom();
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
            var asyncOp = Addressables.LoadAssetAsync<Sprite>(assetReference);
            await new WaitUntil(() => asyncOp.IsDone);
            return asyncOp.Result;
        }

        public void ReturnAsset(AssetReference asset)
        {
            loadedScreens.Remove(asset);
            Addressables.Release(asset);
        }
    }
}
