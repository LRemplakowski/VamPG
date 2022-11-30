using Redcode.Awaiting;
using SunsetSystems.Resources;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SFXController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _sfxSource;

        [SerializeField]
        private List<AudioClip> _typewriterLoop;
        [SerializeField]
        private AudioClip _typewriterEndBell;

        private static System.Random _random = new();

        public bool DoPlayTypewriterLoop { get; set; }

        private void Awake()
        {
            _sfxSource ??= GetComponent<AudioSource>();
        }

        public void PlayLooped(string sfxName)
        {
            AudioClip clip = ResourceLoader.GetSFX(sfxName);
            _sfxSource.clip = clip;
            _sfxSource.loop = true;
            _sfxSource.Play();
        }

        public void PlayOneShot(string sfxName)
        {
            AudioClip clip = ResourceLoader.GetSFX(sfxName);
            PlayOneShot(clip);
        }

        public void PlayOneShot(AudioClip clip)
        {
            _sfxSource.loop = false;
            _sfxSource.PlayOneShot(clip);
        }

        public void PlayTyperwriterLoop()
        {
            DoPlayTypewriterLoop = true;
            PlayTyperwriterSFX();
        }

        private async void PlayTyperwriterSFX()
        {
            AudioClip clip = _typewriterLoop[_random.Next(0, _typewriterLoop.Count)];
            if (DoPlayTypewriterLoop)
            {
                PlayOneShot(clip);
                await new WaitForSeconds(clip.length);
                _sfxSource.Stop();
                PlayTyperwriterSFX();
            }
        }

        public void StopTyperwriterLoop()
        {
            _sfxSource.Stop();
            _sfxSource.loop = false;
            DoPlayTypewriterLoop = false;
            PlayOneShot(_typewriterEndBell);
        }

        public void StopSFX()
        {
            _sfxSource.loop = false;
            _sfxSource.Stop();
        }
    }
}
