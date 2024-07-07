using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.DynamicLog
{
    public class DynamicLogUI : MonoBehaviour
    {
        [Title("Config")]
        [SerializeField]
        private int _maxLogEntryCount = 20;
        [Title("References")]
        [SerializeField, Required]
        private Transform _logEntryParent;
        [SerializeField, Required]
        private TextLogItem _logEntryPrefab;

        private Queue<GameObject> textItems;

        private void Start()
        {
            textItems = new();
        }


        public void LogText(LogEntryData entryData)
        {
            if (textItems.Count >= _maxLogEntryCount)
            {
                GameObject lastEntry = textItems.Dequeue();
                Destroy(lastEntry);
            }

            var newText = Instantiate(_logEntryPrefab, _logEntryParent);
            newText.SetText(entryData);
            textItems.Enqueue(newText.gameObject);
        }


    }
}
