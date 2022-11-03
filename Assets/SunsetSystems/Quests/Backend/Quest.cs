using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Sunset Journal/Quest")]
    public class Quest : ScriptableObject
    {
        [field: SerializeField, ReadOnly]
        public string ID { get; private set; }
        [field: SerializeField, AllowNesting]
        public QuestData Data { get; private set; }

        public event Action<Quest> OnQuestStarted;
        public event Action<Quest> OnQuestCompleted;

        public void LinkAndInitializeObjectives()
        {
            List<Objective> objectives = Data.Objectives;
            if (objectives.Count <= 0)
                return;
            for (int i = objectives.Count - 1; i > 0; i--)
            {
                Objective next = objectives[i];
                objectives[i - 1].NextObjective = next;
                next.IsLast = false;
                next.IsFirst = false;
                if (string.IsNullOrEmpty(next.ID))
                    next.ID = $"Objective {i}";
            }
            Objective last = objectives[^1];
            last.IsLast = true;
            last.IsFirst = false;
            Objective first = objectives[0];
            first.ID = "Objective 0";
            if (last.Equals(first))
            {
                last.IsFirst = true;
            }
            else
            {
                first.IsFirst = true;
                first.IsLast = false;
            }
        }

        private void OnValidate()
        {
            LinkAndInitializeObjectives();
        }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(ID))
                ID = Guid.NewGuid().ToString();
            if (QuestDatabase.Instance.IsRegistered(this) == false)
                QuestDatabase.Instance.RegisterQuest(this);
        }

        public void Begin()
        {
            OnQuestStarted?.Invoke(this);
        }

        public void Complete()
        {
            OnQuestCompleted?.Invoke(this);
        }
    }

    [Serializable]
    public struct QuestData
    {
        public string Name;
        public QuestCategory Category;
        [TextArea(10, 15)]
        public string Description;
        [ReorderableList, AllowNesting]
        public List<Objective> Objectives;
    }

    public enum QuestCategory
    {
        Main, Side, Case
    }
}
