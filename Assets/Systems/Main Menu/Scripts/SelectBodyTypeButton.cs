using SunsetSystems.GameData;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.MainMenu.UI
{
    public class SelectBodyTypeButton : MainMenuNavigationButton
    {
        private GameStarter gameInitializer;
        [SerializeField]
        private BodyType associatedBodyType;

        protected override void Start()
        {
            base.Start();
            if (gameInitializer == null)
                gameInitializer = FindObjectOfType<GameStarter>();
        }

        public override void OnClick()
        {
            gameInitializer.SelectBodyType(associatedBodyType);
            base.OnClick();
        }
    }
}
