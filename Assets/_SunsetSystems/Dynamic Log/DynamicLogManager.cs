using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.DynamicLog
{
    public class DynamicLogManager : SerializedMonoBehaviour
    {
        public static DynamicLogManager Instance { get; private set; }

        [Title("References")]
        [SerializeField]
        private DynamicLogUI _logUI;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        [Title("Editor Utility")]
        [Button]
        public void PostLogMessage(string message)
        {
            LogEntryData messageData = new()
            {
                PlainText = message
            };
            ProcessMessageMetadata(ref messageData);
            _logUI.LogText(messageData);
        }

        private void ProcessMessageMetadata(ref LogEntryData messageData)
        {
            // Future proofing, let's just post plain text for now
            messageData.FormattedText = messageData.PlainText;
        }
    }
}
