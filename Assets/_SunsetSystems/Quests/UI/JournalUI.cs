using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Journal.UI
{
    public class JournalUI : MonoBehaviour
    {
        [SerializeField, Required]
        private QuestListing _mainQuests, _sideQuests, _caseQuests, _completedQuests;

        private void OnEnable()
        {
            _mainQuests.ListQuests(QuestJournal.Instance.MainQuests);
            _sideQuests.ListQuests(QuestJournal.Instance.SideQuests);
            _caseQuests.ListQuests(QuestJournal.Instance.CaseQuests);
            _completedQuests.ListQuests(QuestJournal.Instance.CompletedQuests);
        }

        private void Start()
        {
            _mainQuests.ViewPool.FirstOrDefault(v => v.gameObject.activeInHierarchy)?.OnButtonClicked();
        }
    }
}
