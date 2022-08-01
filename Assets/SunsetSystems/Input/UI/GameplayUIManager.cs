using SunsetSystems.UI.Pause;
using SunsetSystems.Utils;
using System;
using UnityEngine;

namespace SunsetSystems.UI
{
    [RequireComponent(typeof(Tagger))]
    public class GameplayUIManager : MonoBehaviour
    {
        [field: SerializeField]
        public InGameUI InGameUI { get; private set; }
        [field: SerializeField]
        public PauseMenuUI PauseMenuUI { get; private set; }
    }
}
