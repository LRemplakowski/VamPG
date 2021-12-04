using UnityEngine;

namespace SunsetSystems.MainMenu.UI
{
    public class SkillDotGroup : ClickableDotGroup
    {
        [SerializeField]
        private SkillType associatedSkill = SkillType.Invalid;
        private GameInitializer gameInitializer;

        private void Start()
        {
            if (!gameInitializer)
                gameInitializer = FindObjectOfType<GameInitializer>();
        }

        public override void OnClick(int fullCount)
        {
            base.OnClick(fullCount);
            gameInitializer.SetSkillValue(associatedSkill, FullDots);
        }
    }
}