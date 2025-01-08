using UnityEngine;

namespace SunsetSystems.CharacterDevelopment
{
    public interface ILearnable
    {
        bool CanAffordToLearn(in ExperienceData expData);

        ref CostData GetLearningCost();
    }

    public readonly struct CostData
    {
        public readonly int PhysicalExpCost;
        public readonly int SocialExpCost;
        public readonly int SkillExpCost;
        public readonly int DisciplineExpCost;
    }
}
