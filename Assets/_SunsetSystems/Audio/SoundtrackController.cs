using Redcode.Awaiting;
using Sirenix.OdinInspector;
using SunsetSystems.Core.SceneLoading;
using SunsetSystems.Game;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundtrackController : SerializedMonoBehaviour
    {
        [SerializeField]
        private AudioSource _soundtrackSource;
        [SerializeField, Range(0.01f, 10f)]
        private float _trackTransitionTime = 1f;
        [SerializeField, DictionaryDrawerSettings(IsReadOnly = true)]
        private Dictionary<GameState, IPlaylist> statePlaylistPairs = new();
        [SerializeField]
        private float _defaultVolume = .5f;
        private float _cachedVolume;
        private float _currentVolume;
        public float Volume
        {
            get
            {
                return _currentVolume;
            }
            set
            {
                _currentVolume = value;
                _soundtrackSource.volume = value;
            }
        }

        private Task _cachedPlaylistTask;
        private IPlaylist _lastPlaylist;
        private bool _playSoundtrack;

        private void Awake()
        {
            _soundtrackSource ??= GetComponent<AudioSource>();
            Volume = _defaultVolume;
            _cachedVolume = Volume;
        }

        private void OnValidate()
        {
            statePlaylistPairs ??= new();
            foreach (GameState state in Enum.GetValues(typeof(GameState)))
            {
                if (statePlaylistPairs.ContainsKey(state) is false)
                    statePlaylistPairs[state] = null;
            }
        }

        public void PlayStatePlaylist(GameState gameState)
        {
            _playSoundtrack = true;
            if (statePlaylistPairs.TryGetValue(gameState, out IPlaylist statePlaylist) && statePlaylist != _lastPlaylist)
            {
                _lastPlaylist = statePlaylist;
                _cachedPlaylistTask = ExecutePlaylist(statePlaylistPairs[gameState]);
            }
        }

        public void StopSoundtrackImmediate()
        {
            _playSoundtrack = false;
            _soundtrackSource.Stop();
            _cachedPlaylistTask = null;
        }

        private async Task ExecutePlaylist(IPlaylist config)
        {
            Task myTask = _cachedPlaylistTask;
            while (_playSoundtrack)
            {
                await FadeOutSource();
                _soundtrackSource.Stop();
                AudioClip newTrack = config.NextTrack();
                _soundtrackSource.clip = newTrack;
                _soundtrackSource.Play();
                await FadeInSource();
                await new WaitForSecondsRealtime(newTrack.length);
                if (_soundtrackSource.clip != null && myTask != _cachedPlaylistTask)
                    break;
            }
        }

        private async Task FadeOutSource()
        {
            float fadeTime = _trackTransitionTime / 2;
            float timeElapsed = 0f;
            _cachedVolume = Volume;
            while (timeElapsed / fadeTime <= 1)
            {
                Volume = Mathf.Clamp01(_cachedVolume - Mathf.Lerp(0, _cachedVolume, timeElapsed / fadeTime));
                timeElapsed += Time.deltaTime;
                await new WaitForUpdate();
            }
        }

        private async Task FadeInSource()
        {
            float fadeTime = _trackTransitionTime / 2;
            float timeElapsed = 0f;
            while (timeElapsed / fadeTime <= 1)
            {
                Volume = Mathf.Clamp01(Mathf.Lerp(0, _cachedVolume, timeElapsed / fadeTime));
                timeElapsed += Time.deltaTime;
                await new WaitForUpdate();
            }
        }

        public void InjectPlaylistData(ScenePlaylistData playlistData)
        {
            if (playlistData.Exploration != null)
                statePlaylistPairs[GameState.Exploration] = playlistData.Exploration;
            if (playlistData.Combat != null)
                statePlaylistPairs[GameState.Combat] = playlistData.Combat;
            if (playlistData.Dialogue != null)
                statePlaylistPairs[GameState.Conversation] = playlistData.Dialogue;
        }
    }
}
