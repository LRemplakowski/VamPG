using SunsetSystems.GameData;
using UnityEngine;

namespace SunsetSystems.MainMenu.UI
{
    public class AttributeDotGroup : ClickableDotGroup
    {
        [SerializeField]
        private AttributeType associatedAttribute = AttributeType.Invalid;
        private GameInitializer gameInitializer;

        protected override void Start()
        {
            base.Start();
            if (!gameInitializer)
                gameInitializer = FindObjectOfType<GameInitializer>();
        }

        public override void OnClick(int fullCount)
        {
            base.OnClick(fullCount);
            gameInitializer.SetAttribueValue(associatedAttribute, FullDots);
        }
    }
}