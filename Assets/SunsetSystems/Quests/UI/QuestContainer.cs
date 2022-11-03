using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SunsetSystems.Journal.UI
{
    public class QuestContainer : MonoBehaviour
    {
        private Quest _quest;
        private QuestView _questView;
        [SerializeField]
        private TextMeshProUGUI _questTitle;

        public void Initialize(Quest _quest, QuestView _questView)
        {
            this._quest = _quest;
            this._questView = _questView;
            _questTitle.text = _quest.Data.Name;
        }

        public void DisplayQuest()
        {
            if (_quest != null && _questView != null)
            {
                _questView.DisplayQuest(_quest);
            }
        }
    }
}
