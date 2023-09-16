using Sirenix.Utilities;
using SunsetSystems.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SunsetSystems.Core.AddressableManagement
{
    public class AddressableManager : Singleton<AddressableManager>
    {
        private readonly Dictionary<AssetReference, AsyncOperationHandle> assetHandlesDictionary = new();

        private void OnDestroy()
        {
            assetHandlesDictionary.Clear();
        }

        public async Task<T> LoadAssetAsync<T>(AssetReferenceT<T> assetReference) where T : UnityEngine.Object
        {
            if (assetHandlesDictionary.TryGetValue(assetReference, out AsyncOperationHandle _))
            {
                return Addressables.LoadAssetAsync<T>(assetReference).Result;
            }
            else
            {
                AsyncOperationHandle<T> asyncOp = Addressables.LoadAssetAsync<T>(assetReference);
                assetHandlesDictionary.Add(assetReference, asyncOp);
                await asyncOp.Task;
                return asyncOp.Result;
            }
        }

        public void ReleaseAsset<T>(AssetReferenceT<T> assetReference) where T : UnityEngine.Object
        {
            if (assetHandlesDictionary.TryGetValue(assetReference, out AsyncOperationHandle handle))
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
                else
                    assetHandlesDictionary.Remove(assetReference);
            }
        }
    }
}
