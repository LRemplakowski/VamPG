using SunsetSystems.Journal;
using SunsetSystems.Party;
using UnityEngine;

namespace SunsetSystems.Experience
{
    [CreateAssetMenu(fileName = "New Experience Reward", menuName = "Sunset Journal/Experience Reward")]
    public class ExperienceReward : ScriptableObject, IRewardable
    {
        [SerializeField]
        private ExperienceType _experienceType;

        public void ApplyReward(int amount)
        {
            PartyManager.Instance.AllCoterieMembers.ForEach(coterieMemberID => ExperienceManager.TryAwardExperience(coterieMemberID, amount, _experienceType));
        }
    }
}
