using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundtrackController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _soundtrackSource;

        private void Awake()
        {
            _soundtrackSource ??= GetComponent<AudioSource>();
        }
    }
}
