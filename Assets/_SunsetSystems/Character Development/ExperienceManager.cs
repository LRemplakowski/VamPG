using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Loading;
using SunsetSystems.Utils;
using System;
using UnityEngine;

namespace SunsetSystems.Experience
{
    [RequireComponent(typeof(UniqueId))]
    public class ExperienceManager : Singleton<ExperienceManager>, ISaveRuntimeData
    {
        [SerializeField]
        private StringExperienceDataDictionary _experienceDataCache = new();

        private UniqueId _unique;

        protected override void Awake()
        {
            base.Awake();
            _unique ??= GetComponent<UniqueId>();
        }

        public static void AddCreatureToExperienceManager(string creatureID)
        {
            bool result = Instance._experienceDataCache.TryAdd(creatureID, new());
            if (result == false)
                Debug.LogError($"Cannot add creature to experience manager. Creature with ID {creatureID} is already tracked!");
        }

        public static bool TryAwardExperience(string creatureID, int amount, ExperienceType experienceType)
        {
            if (Instance._experienceDataCache.TryGetValue(creatureID, out ExperienceData data))
            {
                data.AddExperience(amount, experienceType);
                Instance._experienceDataCache[creatureID] = data;
            }
            return false;
        }

        public static bool TryRemoveExperience(string creatureID, int amount, ExperienceType experienceType)
        {
            if (Instance._experienceDataCache.TryGetValue(creatureID, out ExperienceData data))
            {
                if (data.GetCurrentExperience(experienceType) < amount)
                    return false;
                data.RemoveExperience(amount, experienceType);
                Instance._experienceDataCache[creatureID] = data;
                return true;
            }
            return false;
        }

        public void SaveRuntimeData()
        {
            ES3.Save(_unique.Id, _experienceDataCache);
        }

        public void LoadRuntimeData()
        {
            _experienceDataCache = ES3.Load<StringExperienceDataDictionary>(_unique.Id);
        }
    }

    public enum ExperienceType
    {
        Physical, Social, Skill, Discipline
    }

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
