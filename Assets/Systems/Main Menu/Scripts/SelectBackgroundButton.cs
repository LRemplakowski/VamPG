using SunsetSystems.GameData;
using Transitions.Manager;
using UnityEngine;

namespace SunsetSystems.MainMenu.UI
{
    public class SelectBackgroundButton : MainMenuNavigationButton
    {
        private GameInitializer gameInitializer;
        [SerializeField]
        private PlayerCharacterBackground associatedBackground;

        protected override void Start()
        {
            base.Start();
            if (gameInitializer == null)
                gameInitializer = FindObjectOfType<GameInitializer>();
        }
        public override void OnClick()
        {
            gameInitializer.SelectBackground(associatedBackground);
            base.OnClick();
        }
    }
}
