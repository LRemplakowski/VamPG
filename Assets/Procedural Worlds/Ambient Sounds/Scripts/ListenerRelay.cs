// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;

namespace AmbientSounds {
    [DisallowMultipleComponent]
    public class ListenerRelay : MonoBehaviour {
        /// <summary> Called by AudioListener or AudioSource to allow script to alter audio graph. </summary>
        /// <param name="data">Array of floats containing current audio graph data.</param>
        /// <param name="channels">Number of audio channels in data.</param>
        private void OnAudioFilterRead(float[] data, int channels)
        {
            AmbienceManager.OnAudioReadInternal(data, channels);
        }

        private void Start()
        {
            AmbienceManager.SetListenerRelay(this);
        }
        private void OnDestroy()
        {
            AmbienceManager.RemoveListenerRelay(this);
        }
    }
}