using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Audio
{
    [Serializable]
    public struct ScenePlaylistData
    {
        [SerializeField]
        public IPlaylist Exploration, Combat, Dialogue;
    }
}
