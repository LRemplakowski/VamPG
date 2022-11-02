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
        [TextArea(10, 15)]
        public string Description;
        [ReorderableList, AllowNesting]
        public List<Objective> Objectives;
    }
}
