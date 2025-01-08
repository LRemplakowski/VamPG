using System;
using SunsetSystems.Journal;
using SunsetSystems.Party;
using UnityEngine;

namespace SunsetSystems.CharacterDevelopment
{
    [CreateAssetMenu(fileName = "New Experience Reward", menuName = "Sunset Journal/Experience Reward")]
    public class ExperienceReward : ScriptableObject, IRewardable
    {
        public delegate void ExperienceUpdate(string recipientID, int amount, ExperienceType experienceType);
        public static event ExperienceUpdate OnExperienceAwared;

        [SerializeField]
        private ExperienceType _experienceType;

        public void ApplyReward(int amount)
        {
            PartyManager.Instance.AllCoterieMembers.ForEach(coterieMemberID => OnExperienceAwared?.Invoke(coterieMemberID, amount, _experienceType));
        }
    }
}
