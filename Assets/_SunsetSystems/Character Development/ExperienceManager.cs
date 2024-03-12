using System;
using System.Collections.Generic;
using CleverCrow.Fluid.UniqueIds;
using Sirenix.OdinInspector;
using SunsetSystems.Persistence;
using UnityEngine;

namespace SunsetSystems.Experience
{
    [RequireComponent(typeof(UniqueId))]
    public class ExperienceManager : SerializedMonoBehaviour, ISaveable
    {
        public static ExperienceManager Instance { get; private set; }

        [SerializeField]
        private Dictionary<string, ExperienceData> _experienceDataCache = new();

        private UniqueId _unique;
        public string DataKey => _unique.Id;

        protected void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
            ISaveable.RegisterSaveable(this);
            _unique ??= GetComponent<UniqueId>();
        }

        private void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
        }

        public void AddCreatureToExperienceManager(string creatureID)
        {
            bool result = _experienceDataCache.TryAdd(creatureID, new());
            if (result == false)
                Debug.LogError($"Cannot add creature to experience manager. Creature with ID {creatureID} is already tracked!");
        }

        public bool TryAwardExperience(string creatureID, int amount, ExperienceType experienceType)
        {
            if (_experienceDataCache.TryGetValue(creatureID, out ExperienceData data))
            {
                data.AddExperience(amount, experienceType);
                _experienceDataCache[creatureID] = data;
            }
            return false;
        }

        public bool TryRemoveExperience(string creatureID, int amount, ExperienceType experienceType)
        {
            if (_experienceDataCache.TryGetValue(creatureID, out ExperienceData data))
            {
                if (data.GetCurrentExperience(experienceType) < amount)
                    return false;
                data.RemoveExperience(amount, experienceType);
                _experienceDataCache[creatureID] = data;
                return true;
            }
            return false;
        }

        public object GetSaveData()
        {
            return new ExperienceSaveData(this);
        }

        public void InjectSaveData(object data)
        {
            if (data is not ExperienceSaveData expData)
                return;
            _experienceDataCache = expData.ExperienceDataCache;
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

        public int GetCurrentExperience(ExperienceType experienceType)
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

        public int GetTotalExperience(ExperienceType experienceType)
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
