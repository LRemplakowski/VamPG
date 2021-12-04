using UnityEngine;

namespace SunsetSystems.MainMenu.UI
{
    public class AttributeDotGroup : ClickableDotGroup
    {
        [SerializeField]
        private AttributeType associatedAttribute = AttributeType.Invalid;
        private GameInitializer gameInitializer;

        private void Start()
        {
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