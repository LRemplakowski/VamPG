using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Audio
{
    [CreateAssetMenu(fileName = "New Playlist Config", menuName = "Sunset Audio/Playlist Config")]
    public class PlaylistConfig : SerializedScriptableObject, IPlaylist
    {
        [SerializeField]
        private List<AudioClip> _tracks = new();

        private int _currentTrackIndex = 0;
        private bool _firstTrackRequest = true;
        

        public AudioClip NextTrack()
        {
            if (_tracks.Count <= 0)
                return default;
            AudioClip track;
            if (_firstTrackRequest)
            {
                track = _tracks[_currentTrackIndex];
                _firstTrackRequest = false;
            }
            else
            {
                _currentTrackIndex = _currentTrackIndex + 1 >= _tracks.Count ? 0 : _currentTrackIndex + 1;
                track = _tracks[_currentTrackIndex];
            }
            return track;
        }

        public AudioClip PreviousTrack()
        {
            if (_tracks.Count <= 0)
                return default;
            AudioClip track;
            if (_firstTrackRequest)
            {
                track = _tracks[_currentTrackIndex];
                _firstTrackRequest = false;
            }
            else
            {
                _currentTrackIndex = _currentTrackIndex - 1 <= 0 ? _tracks.Count - 1 : _currentTrackIndex - 1;
                track = _tracks[_currentTrackIndex];
            }
            return track;
        }

        public AudioClip GetCurrentTrack()
        {
            return _tracks[_currentTrackIndex];
        }
    }
}
