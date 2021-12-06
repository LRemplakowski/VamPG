using SunsetSystems.Journal;
using UnityEngine;

namespace SunsetSystems.MainMenu.UI
{
    public class SkillDotGroup : ClickableDotGroup
    {
        [SerializeField]
        private SkillType associatedSkill = SkillType.Invalid;
        private GameInitializer gameInitializer;

        protected override void Start()
        {
            if (!gameInitializer)
                gameInitializer = FindObjectOfType<GameInitializer>();
            base.Start();
        }

        public override void OnClick(int fullCount)
        {
            base.OnClick(fullCount);
            if (gameInitializer)
                gameInitializer.SetSkillValue(associatedSkill, FullDots);
        }
    }
}