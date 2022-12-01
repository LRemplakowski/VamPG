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

        private bool _playSoundtrack;

        private void Awake()
        {
            _soundtrackSource ??= GetComponent<AudioSource>();
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
            while (timeElapsed / fadeTime <= 1)
            {
                float volume = Mathf.Clamp01(1f - Mathf.Lerp(0, 1, timeElapsed / fadeTime));
                timeElapsed += Time.deltaTime;
                _soundtrackSource.volume = volume;
                await new WaitForUpdate();
            }
        }

        private async Task FadeInSource()
        {
            float fadeTime = _trackTransitionTime / 2;
            float timeElapsed = 0f;
            while (timeElapsed / fadeTime <= 1)
            {
                float volume = Mathf.Clamp01(Mathf.Lerp(0, 1, timeElapsed / fadeTime));
                timeElapsed += Time.deltaTime;
                _soundtrackSource.volume = volume;
                await new WaitForUpdate();
            }
        }
    }
}
