using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Game;
using UnityEngine;

namespace SunsetSystems.Audio
{
    public class PlaylistOverride : SerializedMonoBehaviour
    {
        [SerializeField]
        private IPlaylist _playlistToInject;
        [SerializeField]
        private GameState _targetGameState;

        public void EnableOverride()
        {
            AudioManager.Instance.SetPlaylistOverride(_targetGameState, _playlistToInject);
        }

        public void DisableOverride()
        {
            AudioManager.Instance.ClearPlaylistOverride(_targetGameState);
        }
    }
}
