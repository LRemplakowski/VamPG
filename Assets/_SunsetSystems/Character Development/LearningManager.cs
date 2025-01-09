using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Sirenix.OdinInspector;
using SunsetSystems.Persistence;
using UnityEngine;

namespace SunsetSystems.CharacterDevelopment
{
    public class LearningManager : SerializedMonoBehaviour, ISaveable
    {
        public string DataKey => DataKeyConstants.LEARNING_MANAGER_DATA_KEY;

        [SerializeField, Required]
        private ExperienceManager _experienceManager;

        public delegate void LearningEvent(string learnerID, ILearnable learnable);
        public static event LearningEvent OnCreatureLearned;

        private static readonly Dictionary<string, HashSet<ILearnable>> _knownLearnablesMap = new();

        private void Awake()
        {
            ISaveable.RegisterSaveable(this);
        }

        private void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
        }

        public IEnumerable<ILearnable> GetKnownLearnables(string creatureID)
        {
            _knownLearnablesMap.TryGetValue(creatureID, out HashSet<ILearnable> result);
            return result;
        }

        public IEnumerable<T> GetKnownLearnables<T>(string creatureID)
        {
            if (_knownLearnablesMap.TryGetValue(creatureID, out var result))
            {
                return result.OfType<T>();
            }
            return default;
        }

        /// <summary>
        /// Tries to learn new ILearnable. Transactional.
        /// </summary>
        /// <param name="creatureID">Creature attemtping to learn.</param>
        /// <param name="learnable">Object to learn.</param>
        /// <returns>Learning successful.</returns>
        public bool TryLearn(string creatureID, ILearnable learnable)
        {
            if (IsKnown(creatureID, learnable))
            {
                return false;
            }
            if (CanAffordToLearn(creatureID, learnable))
            {
                return DoLearn(creatureID, learnable);
            }
            return false;
        }

        /// <summary>
        /// Tries to forget alreadny known ILearnable. Returns experience spent. Transactional.
        /// </summary>
        /// <param name="creatureID">Creature attempting to unlearn.</param>
        /// <param name="learnable">Object to unlearn.</param>
        /// <returns>Unlearning successful.</returns>
        public bool TryUnlearn(string creatureID, ILearnable learnable)
        {
            if (IsKnown(creatureID, learnable))
            {
                return DoUnlearn(creatureID, learnable);
            }
            return false;
        }

        private bool DoUnlearn(string creatureID, ILearnable learnable)
        {
            var learnables = _knownLearnablesMap[creatureID];
            ref var learnableCost = ref learnable.GetLearningCost();
            try
            {
                using TransactionScope transactionScope = new();
                RemoveLearnable(learnables, learnable);
                RegainExperience(_experienceManager, creatureID, in learnableCost);
                transactionScope.Complete();
                return true;
            }
            catch (TransactionAbortedException exception)
            {
                Debug.LogException(exception);
                return false;
            }
        }

        private bool DoLearn(string creatureID, ILearnable learnable)
        {
            var learnables = _knownLearnablesMap[creatureID];
            ref var learnableCost = ref learnable.GetLearningCost();
            try
            {
                using TransactionScope transactionScope = new();
                SpendExperience(_experienceManager, creatureID, in learnableCost);
                AddLearnable(learnables, learnable);
                transactionScope.Complete();
                OnCreatureLearned?.Invoke(creatureID, learnable);
                return true;
            }
            catch (TransactionAbortedException exception)
            {
                Debug.LogException(exception);
                return false;
            }
        }

        public bool IsKnown(string creatureID, ILearnable learnable)
        {
            return _knownLearnablesMap.TryGetValue(creatureID, out var knownLearnables) && knownLearnables.Contains(learnable);
        }

        public bool CanAffordToLearn(string creatureID, ILearnable learnable)
        {
            if (_experienceManager.TryGetExperienceData(creatureID, out var experienceData))
            {
                return learnable.CanAffordToLearn(in experienceData);
            }
            return false;
        }

        private static void AddLearnable(HashSet<ILearnable> learnables, ILearnable toAdd)
        {
            if (learnables.Add(toAdd) is false)
            {
                throw new TransactionAbortedException($"ExperienceManager >>> Learnable {toAdd} is already known! Cannot be learned!");
            }
        }

        private static void RemoveLearnable(HashSet<ILearnable> learnables, ILearnable toRemove)
        {
            if (learnables.Remove(toRemove))
            {
                throw new TransactionAbortedException($"ExperienceManager >>> Learnable {toRemove} is not known! Cannot be unlearned!");
            }
        }

        private static void SpendExperience(ExperienceManager experienceManager, string creatureID, in CostData learningCost)
        {
            if (experienceManager.TryRemoveExperience(creatureID, learningCost.PhysicalExpCost, ExperienceType.Physical) is false)
            {
                throw new TransactionAbortedException("ExperienceManager >>> Failed to spend Physical Exp!");
            }
            if (experienceManager.TryRemoveExperience(creatureID, learningCost.SocialExpCost, ExperienceType.Social) is false)
            {
                throw new TransactionAbortedException("ExperienceManager >>> Failed to spend Social Exp!");
            }
            if (experienceManager.TryRemoveExperience(creatureID, learningCost.SkillExpCost, ExperienceType.Skill) is false)
            {
                throw new TransactionAbortedException("ExperienceManager >>> Failed to spend Skill Exp!");
            }
            if (experienceManager.TryRemoveExperience(creatureID, learningCost.DisciplineExpCost, ExperienceType.Discipline) is false)
            {
                throw new TransactionAbortedException("ExperienceManager >>> Failed to spend Discipline Exp!");
            }
        }

        private static void RegainExperience(ExperienceManager experienceManager, string creatureID, in CostData learningCost)
        {
            if (experienceManager.TryAwardExperience(creatureID, learningCost.PhysicalExpCost, ExperienceType.Physical) is false)
            {
                throw new TransactionAbortedException("ExperienceManager >>> Failed to regain Physical Exp!");
            }
            if (experienceManager.TryAwardExperience(creatureID, learningCost.SocialExpCost, ExperienceType.Social) is false)
            {
                throw new TransactionAbortedException("ExperienceManager >>> Failed to regain Social Exp!");
            }
            if (experienceManager.TryAwardExperience(creatureID, learningCost.SkillExpCost, ExperienceType.Skill) is false)
            {
                throw new TransactionAbortedException("ExperienceManager >>> Failed to regain Skill Exp!");
            }
            if (experienceManager.TryAwardExperience(creatureID, learningCost.DisciplineExpCost, ExperienceType.Discipline) is false)
            {
                throw new TransactionAbortedException("ExperienceManager >>> Failed to regain Discipline Exp!");
            }
        }

        public object GetSaveData()
        {
            return new LearningData(this);
        }

        public bool InjectSaveData(object data)
        {
            if (data is not LearningData learningData)
                return false;
            _knownLearnablesMap.Clear();
            return true;
        }

        public class LearningData : SaveData
        {
            public LearningData(LearningManager learningManager) : base()
            {

            }
        }
    }
}
