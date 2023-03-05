using SunsetSystems.UI.Utils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SunsetSystems.Journal.UI
{
    public class QuestContainer : MonoBehaviour, IUserInterfaceView<Quest>
    {
        private Quest _quest;
        [SerializeField]
        private TextMeshProUGUI _questTitle;
        [SerializeField]
        private Button _button;

        public static event Action<Quest> QuestSelectorButtonClicked;

        private IQuestJournal _questJournal;

        [Inject]
        public void InjectDependencies(IQuestJournal questJournal)
        {
            _questJournal = questJournal;
        }

        public void OnButtonClicked()
        {
            QuestSelectorButtonClicked?.Invoke(_quest);
        }

        public void UpdateView(IGameDataProvider<Quest> dataProvider)
        {
            _quest = dataProvider.Data;
            _questTitle.text = _quest.Name;
            _button.interactable = !_questJournal.IsQuestCompleted(_quest.ID);
        }
    }
}
