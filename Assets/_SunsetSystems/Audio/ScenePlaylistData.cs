using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Audio
{
    [Serializable]
    public struct ScenePlaylistData
    {
        [SerializeField, AssetsOnly, LabelWidth(100)]
        public IPlaylist Exploration, Combat, Dialogue;
    }
}
