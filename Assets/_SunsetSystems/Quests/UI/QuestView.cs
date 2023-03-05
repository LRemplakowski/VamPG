using SunsetSystems.Journal;
using SunsetSystems.Journal.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SunsetSystems
{
    public class QuestView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _title, _description;
        [SerializeField]
        private Image _questgiverPortrait, _questArea;
        [SerializeField]
        private TextMeshProUGUI _objectivePoolElementPrefab;
        [SerializeField]
        private Transform _objectivePoolParent;
        [SerializeField]
        private List<TextMeshProUGUI> _objectivePool = new();

        private Quest _cachedQuest;

        private IQuestJournal _questJournal;

        [Inject]
        public void InjectDependencies(IQuestJournal questJournal)
        {
            _questJournal = questJournal;
        }

        private void Awake()
        {
            _objectivePool = new();
        }

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
            _objectivePool.RemoveAll(obView => obView == null);
            _objectivePool.ForEach(obView => obView.gameObject.SetActive(false));
            if (_questJournal.GetCurrentObjectives(_cachedQuest.ID, out List<Objective> objectives))
            {
                foreach (Objective objective in objectives)
                {
                    Debug.Log("Setting objective view for objective " + objective.Description);
                    TextMeshProUGUI objectiveView = GetObjectiveViewFromPool();
                    objectiveView.text = objective.Description;
                    objectiveView.gameObject.SetActive(true);
                }
            }
        }

        private TextMeshProUGUI GetObjectiveViewFromPool()
        {
            TextMeshProUGUI view = default;
            view = _objectivePool.FirstOrDefault(obView => obView.IsActive() == false);
            if (view == null)
            {
                view = Instantiate(_objectivePoolElementPrefab, _objectivePoolParent);
                view.gameObject.SetActive(false);
                _objectivePool.Add(view);
            }
            return view;
        }
    }
}
