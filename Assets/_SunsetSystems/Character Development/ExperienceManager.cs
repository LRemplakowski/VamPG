using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Persistence;
using UnityEngine;

namespace SunsetSystems.CharacterDevelopment
{
    public class ExperienceManager : SerializedMonoBehaviour, ISaveable
    {
        public static ExperienceManager Instance { get; private set; }

        [SerializeField]
        private Dictionary<string, ExperienceData> _experienceDataCache = new();

       
        public string DataKey => DataKeyConstants.EXPERIENCE_MANAGER_DATA_KEY;

        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            ExperienceReward.OnExperienceAwared += OnExperienceAwarded;
            ISaveable.RegisterSaveable(this);
            
        }

        private void OnDestroy()
        {
            ExperienceReward.OnExperienceAwared -= OnExperienceAwarded;
            ISaveable.UnregisterSaveable(this);
        }

        private void OnExperienceAwarded(string recipientID, int amount, ExperienceType experienceType)
        {
            if (TryAwardExperience(recipientID, amount, experienceType))
            {
                Debug.Log($"ExperienceManager >>> Awarded {recipientID} with {amount} {experienceType} experience.");
            }
            else
            {
                Debug.LogError($"ExperienceManager >>> Failed to award experience to {recipientID}!");
            }
        }

        public void AddCreatureToExperienceManager(string creatureID)
        {
            if (string.IsNullOrWhiteSpace(creatureID))
            {
                Debug.LogError($"ExperienceManager >>> Failed to add Creature! Given null or whitespace ID!");
                return;
            }
            if (_experienceDataCache.ContainsKey(creatureID))
            {
                Debug.LogError($"ExperienceManager >>> Cannot add Creature! Creature is already tracked.");
                return;
            }
            _experienceDataCache[creatureID] = new();
        }

        public bool TryAwardExperience(string creatureID, int amount, ExperienceType experienceType)
        {
            if (_experienceDataCache.TryGetValue(creatureID, out ExperienceData data))
            {
                data.AddExperience(amount, experienceType);
                _experienceDataCache[creatureID] = data;
                return true;
            }
            return false;
        }

        public bool TryRemoveExperience(string creatureID, int amount, ExperienceType experienceType)
        {
            if (_experienceDataCache.TryGetValue(creatureID, out ExperienceData data))
            {
                if (amount <= 0)
                    return true;
                if (data.GetCurrentExperience(experienceType) < amount)
                    return false;
                data.RemoveExperience(amount, experienceType);
                _experienceDataCache[creatureID] = data;
                return true;
            }
            return false;
        }

        public bool TryGetExperienceData(string creatureID, out ExperienceData experienceData)
        {
            return _experienceDataCache.TryGetValue(creatureID, out experienceData);
        }

        public object GetSaveData()
        {
            return new ExperienceSaveData(this);
        }

        public bool InjectSaveData(object data)
        {
            if (data is not ExperienceSaveData expData)
                return false;
            _experienceDataCache = expData.ExperienceDataCache;
            return true;
        }

        [Serializable]
        private class ExperienceSaveData : SaveData
        {
            public Dictionary<string, ExperienceData> ExperienceDataCache;

            public ExperienceSaveData(ExperienceManager manager)
            {
                ExperienceDataCache = manager._experienceDataCache;
            }

            public ExperienceSaveData()
            {
                ExperienceDataCache = new();
            }
        }
    }

    public enum ExperienceType
    {
        Physical, Social, Skill, Discipline
    }

    [Serializable]
    public struct ExperienceData
    {
        public int Physical, PhysicalTotal;
        public int Social, SocialTotal;
        public int Skill, SkillTotal;
        public int Discipline, DisciplineTotal;

        public void AddExperience(int amount, ExperienceType experienceType)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException($"Experience amount added cannot be less then 0! Value provided = {amount}");
            switch (experienceType)
            {
                case ExperienceType.Physical:
                    Physical += amount;
                    PhysicalTotal += amount;
                    break;
                case ExperienceType.Social:
                    Social += amount;
                    SocialTotal += amount;
                    break;
                case ExperienceType.Skill:
                    Skill += amount;
                    SkillTotal += amount;
                    break;
                case ExperienceType.Discipline:
                    Discipline += amount;
                    DisciplineTotal += amount;
                    break;
            }
        }

        public void RemoveExperience(int amount, ExperienceType experienceType)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException($"Experience amount removed cannot be less then 0! Value provided = {amount}");
            switch (experienceType)
            {
                case ExperienceType.Physical:
                    Physical -= amount;
                    break;
                case ExperienceType.Social:
                    Social -= amount;
                    break;
                case ExperienceType.Skill:
                    Skill -= amount;
                    break;
                case ExperienceType.Discipline:
                    Discipline -= amount;
                    break;
            }
        }

        public readonly int GetCurrentExperience(ExperienceType experienceType)
        {
            return experienceType switch
            {
                ExperienceType.Physical => Physical,
                ExperienceType.Social => Social,
                ExperienceType.Skill => Skill,
                ExperienceType.Discipline => Discipline,
                _ => 0,
            };
        }

        public readonly int GetTotalExperience(ExperienceType experienceType)
        {
            return experienceType switch
            {
                ExperienceType.Physical => PhysicalTotal,
                ExperienceType.Social => SocialTotal,
                ExperienceType.Skill => SkillTotal,
                ExperienceType.Discipline => DisciplineTotal,
                _ => 0,
            };
        }
    }
}
