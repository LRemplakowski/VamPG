using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Audio
{
    public interface IPlaylist
    {
        AudioClip NextTrack();
        AudioClip PreviousTrack();
        AudioClip GetCurrentTrack();
    }
}
