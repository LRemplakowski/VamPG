using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Audio
{
    public interface IAudioManager
    {
        float MusicDefaultVolume { get; }
        float SFXDefaultVolume { get; }

        void PlaySFXOneShot(string sfxName);
        void PlaySFXOneShot(AudioClip clip);
        void PlayTyperwriterLoop();
        void PlayTypewriterEnd();
        void StopSFXPlayback();
        void PlayMenuMusic();
        void PlayGameplayMusic();
        void SetMusicVolume(float volume);
        void SetSFXVolume(float volume);
    }
}
