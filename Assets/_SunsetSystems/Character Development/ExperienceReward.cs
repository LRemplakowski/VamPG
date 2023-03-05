using SunsetSystems.Journal;
using SunsetSystems.Party;
using UnityEngine;
using Zenject;

namespace SunsetSystems.Experience
{
    [CreateAssetMenu(fileName = "New Experience Reward", menuName = "Sunset Journal/Experience Reward")]
    public class ExperienceReward : ScriptableObject, IRewardable
    {
        [SerializeField]
        private ExperienceType _experienceType;

        private IPartyManager _partyManager;
        private IExperienceManager _experienceManager;

        [Inject]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void InjectDependencies(IPartyManager partyManager, IExperienceManager experienceManager)
        {
            _partyManager = partyManager;
            _experienceManager = experienceManager;
        }

        public void ApplyReward(int amount)
        {
            _partyManager.AllCoterieMembers.ForEach(cd => _experienceManager.TryAwardExperience(cd.ID, amount, _experienceType));
        }
    }
}
