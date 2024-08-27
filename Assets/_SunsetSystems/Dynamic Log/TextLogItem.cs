using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace SunsetSystems.DynamicLog
{
    public class TextLogItem : MonoBehaviour
    {
        [SerializeField, Required]
        private TextMeshProUGUI _logText;

        public void SetText(LogEntryData entryData)
        {
            _logText.text = entryData.FormattedText;
        }
    }
}
