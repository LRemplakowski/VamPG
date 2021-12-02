using System.Collections;
using System.Collections.Generic;
using Systems.Journal;
using UnityEngine;

namespace Systems.MainMenu
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private PlayerCharacterBackground selectedBackground;
        [SerializeField, ReadOnly]
        private BodyType selectedBodyType;

        public void SelectBackground(PlayerCharacterBackground selectedBackground)
        {
            this.selectedBackground = selectedBackground;
        }

        public void SelectBodyType(BodyType selectedBodyType)
        {
            this.selectedBodyType = selectedBodyType;
        }
    }
}
