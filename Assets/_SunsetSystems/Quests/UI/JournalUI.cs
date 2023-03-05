using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace SunsetSystems.Journal.UI
{
    public class JournalUI : MonoBehaviour
    {
        [SerializeField, Required]
        private QuestListing _mainQuests, _sideQuests, _caseQuests, _completedQuests;

        private IQuestJournal _questJournal;

        [Inject]
        public void InjectDependencies(IQuestJournal questJournal)
        {
            _questJournal = questJournal;
        }

        private void OnEnable()
        {
            _mainQuests.ListQuests(_questJournal.MainQuests);
            _sideQuests.ListQuests(_questJournal.SideQuests);
            _caseQuests.ListQuests(_questJournal.CaseQuests);
            _completedQuests.ListQuests(_questJournal.CompletedQuests);
        }

        private void Start()
        {
            _mainQuests.ViewPool.FirstOrDefault(v => v.gameObject.activeInHierarchy)?.OnButtonClicked();
        }
    }
}
