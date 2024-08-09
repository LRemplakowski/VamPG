using Redcode.Awaiting;
using SunsetSystems.Journal;
using SunsetSystems.Journal.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [SerializeField]
        private TextMeshProUGUI _objectivePoolElementPrefab;
        [SerializeField]
        private Transform _objectivePoolParent;
        [SerializeField]
        private List<TextMeshProUGUI> _objectivePool = new();

        private Quest _cachedQuest;

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
        
        private async void UpdateQuestView()
        {
            if (_cachedQuest == null)
                return;
            _title.text = _cachedQuest.Name;
            _description.text = _cachedQuest.Description;
            _objectivePool.RemoveAll(obView => obView == null);
            _objectivePool.ForEach(obView => obView.gameObject.SetActive(false));
            if (QuestJournal.Instance == null)
                await new WaitUntil(() => QuestJournal.Instance != null);
            if (QuestJournal.Instance.GetCurrentObjectives(_cachedQuest.DatabaseID, out List<Objective> objectives))
            {
                foreach (Objective objective in objectives)
                {
                    Debug.Log("Setting objective view for objective " + objective.Description);
                    TextMeshProUGUI objectiveView = await GetObjectiveViewFromPool();
                    objectiveView.text = objective.Description;
                    objectiveView.gameObject.SetActive(true);
                }
            }
        }

        private async Task<TextMeshProUGUI> GetObjectiveViewFromPool()
        {
            TextMeshProUGUI view = default;
            if (_objectivePool == null)
                _objectivePool = new();
            if (_objectivePool.Count > 0)
                view = _objectivePool.FirstOrDefault(obView => obView != null && obView.IsActive() == false);
            if (view == null)
            {
                if (_objectivePoolElementPrefab == null)
                {
                    Debug.LogError("Quest view has a null prefab! Fix it you dummy!");
                    GameObject newOb = new();
                    view = newOb.AddComponent<TextMeshProUGUI>();
                    _objectivePool.Add(view);
                    return view;
                } 
                else if (_objectivePoolParent == null)
                {
                    Debug.LogError("Quest View has a null objective pool parent! Waiting...");
                    await new WaitUntil(() => _objectivePoolParent != null || Task.Delay(10000).IsCompleted);
                    if (_objectivePoolParent == null)
                        _objectivePoolParent = new GameObject().transform;
                }
                view = Instantiate(_objectivePoolElementPrefab, _objectivePoolParent);
                view.gameObject.SetActive(false);
                _objectivePool.Add(view);
            }
            return view;
        }
    }
}
