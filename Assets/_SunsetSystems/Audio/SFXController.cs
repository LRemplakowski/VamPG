using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SFXController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _sfxSource;

        private void Awake()
        {
            _sfxSource ??= GetComponent<AudioSource>();
        }
    }
}
