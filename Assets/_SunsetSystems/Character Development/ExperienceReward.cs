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
            PartyManager.AllCoterieMembers.ForEach(cd => ExperienceManager.TryAwardExperience(cd.ID, amount, _experienceType));
        }
    }
}
