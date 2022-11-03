using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal.UI
{
    public class JournalUI : MonoBehaviour
    {
        [SerializeField, Required]
        private QuestListing _mainQuests, _sideQuests, _caseQuests, _completedQuests;
        [SerializeField,Required]
        private QuestView _questView;

        private void OnEnable()
        {
            _mainQuests.ListQuests(QuestJournal.Instance.MainQuests, _questView);
            _sideQuests.ListQuests(QuestJournal.Instance.SideQuests, _questView);
            _caseQuests.ListQuests(QuestJournal.Instance.CaseQuests, _questView);
            _completedQuests.ListQuests(QuestJournal.Instance.CompletedQuests, _questView);
        }
    }
}
