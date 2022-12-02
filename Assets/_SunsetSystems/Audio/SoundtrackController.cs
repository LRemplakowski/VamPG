using Redcode.Awaiting;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundtrackController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _soundtrackSource;
        [SerializeField, Range(0.01f, 10f)]
        private float _trackTransitionTime = 1f;
        [SerializeField]
        private PlaylistConfig _menuPlaylist, _gamePlaylist;
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

        private bool _playSoundtrack;

        private void Awake()
        {
            _soundtrackSource ??= GetComponent<AudioSource>();
            Volume = _defaultVolume;
            _cachedVolume = Volume;
        }

        public void PlayMenuPlaylist()
        {
            _playSoundtrack = true;
            ExecutePlaylist(_menuPlaylist);
        }

        public void PlayGamePlaylist()
        {
            _playSoundtrack = true;
            ExecutePlaylist(_gamePlaylist);
        }

        public void StopSoundtrackImmediate()
        {
            _playSoundtrack = false;
            _soundtrackSource.Stop();
        }

        private async void ExecutePlaylist(PlaylistConfig config)
        {
            while (_playSoundtrack)
            {
                await FadeOutSource();
                _soundtrackSource.Stop();
                AudioClip newTrack = config.NextTrack();
                _soundtrackSource.clip = newTrack;
                _soundtrackSource.Play();
                await FadeInSource();
                await new WaitForSecondsRealtime(newTrack.length);
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
    }
}
