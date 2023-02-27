using SunsetSystems.Data;
using System.Threading.Tasks;
using SunsetSystems.LevelManagement;
using UnityEngine;

namespace SunsetSystems.MainMenu.UI
{
    public class SelectBackgroundButton : MainMenuNavigationButton
    {
        private GameStarter gameInitializer;
        [SerializeField]
        private PlayerCharacterBackground associatedBackground;

        protected override void Start()
        {
            base.Start();
            if (gameInitializer == null)
                gameInitializer = FindObjectOfType<GameStarter>();
        }
        public override void OnClick()
        {
            gameInitializer.SelectBackground(associatedBackground);
            base.OnClick();
        }
    }
}
