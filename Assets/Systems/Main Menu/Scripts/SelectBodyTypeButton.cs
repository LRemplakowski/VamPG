using SunsetSystems.GameData;
using UnityEngine;

namespace SunsetSystems.MainMenu.UI
{
    public class SelectBodyTypeButton : MainMenuNavigationButton
    {
        private GameInitializer gameInitializer;
        [SerializeField]
        private BodyType associatedBodyType;

        protected override void Start()
        {
            base.Start();
            if (gameInitializer == null)
                gameInitializer = FindObjectOfType<GameInitializer>();
        }

        public override void OnClick()
        {
            gameInitializer.SelectBodyType(associatedBodyType);
            base.OnClick();
        }
    }
}
