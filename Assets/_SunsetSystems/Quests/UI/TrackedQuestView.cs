using SunsetSystems.UI.Utils;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Journal.UI
{
    public class TrackedQuestView : MonoBehaviour, IUserInterfaceView<Quest>
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TextMeshProUGUI _title;
        [SerializeField]
        private TextMeshProUGUI _objectivePrefab;
        [SerializeField]
        private Transform _objectivePoolParent;

        private List<TextMeshProUGUI> _textPool = new();

        public void UpdateView(IGameDataProvider<Quest> dataProvider)
        {
            _textPool.ForEach(text => text.gameObject.SetActive(false));
            _title.text = dataProvider.Data.Name;
            if (QuestJournal.Instance.GetCurrentObjectives(dataProvider.Data.ID, out List<Objective> objectives))
            {
                foreach (Objective objective in objectives)
                {
                    if (string.IsNullOrWhiteSpace(objective.Description) == false)
                    {
                        TextMeshProUGUI textObject = GetTMPFromPool();
                        textObject.text = objective.Description;
                        textObject.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                Debug.LogError($"No objectives for quest {dataProvider.Data.Name}!");
            }
        }

        private TextMeshProUGUI GetTMPFromPool()
        {
            foreach (TextMeshProUGUI text in _textPool)
            {
                if (text != null && text.IsActive() == false)
                    return text;
            }
            TextMeshProUGUI newText = Instantiate(_objectivePrefab, _objectivePoolParent);
            newText.gameObject.SetActive(false);
            _textPool.Add(newText);
            return newText;
        }
    }
}
