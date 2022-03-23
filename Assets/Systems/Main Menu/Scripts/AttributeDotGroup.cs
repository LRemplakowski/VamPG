using SunsetSystems.Data;
using UnityEngine;

namespace SunsetSystems.MainMenu.UI
{
    public class AttributeDotGroup : ClickableDotGroup
    {
        [SerializeField]
        private AttributeType associatedAttribute = AttributeType.Invalid;
        private GameStarter gameInitializer;

        protected override void Start()
        {
            base.Start();
            if (!gameInitializer)
                gameInitializer = FindObjectOfType<GameStarter>();
        }

        public override void OnClick(int fullCount)
        {
            base.OnClick(fullCount);
            gameInitializer.SetAttribueValue(associatedAttribute, FullDots);
        }
    }
}