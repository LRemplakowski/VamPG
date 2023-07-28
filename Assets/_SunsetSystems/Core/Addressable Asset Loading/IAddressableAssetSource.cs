using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Core.AddressableManagement
{
    public interface IAddressableAssetSource<T> : IAddressableAssetSource where T : UnityEngine.Object
    {
        Task<T> GetAssetAsync(AssetReferenceT<T> assetReference);
        void ReturnAsset(AssetReferenceT<T> asset);
    }

    public interface IAddressableAssetSource
    {

    }
}
