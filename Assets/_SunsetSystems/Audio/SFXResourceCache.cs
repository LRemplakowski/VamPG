using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Audio
{
    [CreateAssetMenu(fileName = "New SFX Cache", menuName = "Sunset Audio/SFX Cache")]
    public class SFXResourceCache : SerializedScriptableObject
    {
        [field: SerializeField]
        public Dictionary<string, AudioClip> SfxAssetCache { get; private set; } = new();
    }
}
