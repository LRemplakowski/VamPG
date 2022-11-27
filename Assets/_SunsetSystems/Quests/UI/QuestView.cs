using SunsetSystems.Journal;
using SunsetSystems.Journal.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems
{
    public class QuestView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _title, _description;
        [SerializeField]
        private Image _questgiverPortrait, _questArea;

        private Quest _cachedQuest;

        private void OnEnable()
        {
            UpdateQuestView();
            QuestContainer.QuestSelectorButtonClicked += OnQuestSelected;
        }

        private void OnDisable()
        {
            QuestContainer.QuestSelectorButtonClicked += OnQuestSelected;
        }

        private void OnQuestSelected(Quest quest)
        {
            if (quest == null)
            {
                Debug.LogError("Trying to display quest in Quest View but the quest is null!");
                return;
            }
            Debug.Log("Displaying quest " + quest.Name);
            _cachedQuest = quest;
            UpdateQuestView();
        }
        
        private void UpdateQuestView()
        {
            if (_cachedQuest == null)
                return;
            _title.text = _cachedQuest.Name;
            _description.text = _cachedQuest.Description;
        }
    }
}
