using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Sunset Journal/Quest")]
    public class Quest : ScriptableObject
    {
        [field: SerializeField, ReadOnly]
        public string ID { get; private set; }
        [field: SerializeField]
        public QuestData Data { get; private set; }

        public event Action OnQuestStarted;
        public event Action OnObjectiveChanged;
        public event Action OnQuestCompleted;

        private void Awake()
        {
            if (string.IsNullOrEmpty(ID))
                ID = Guid.NewGuid().ToString();
            if (QuestDatabase.instance == null)
            {
                Debug.LogError("There is no quest database!");
                return;
            }
            QuestDatabase.instance.RegisterQuest(this);
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(ID))
                ID = Guid.NewGuid().ToString();
        }
    }

    [Serializable]
    public struct QuestData
    {
        public string Name;
    }
}
