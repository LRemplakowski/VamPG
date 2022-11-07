using SunsetSystems.UI.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Journal.UI
{
    public class QuestContainer : MonoBehaviour, IUserInterfaceView<Quest, QuestContainer>
    {
        private Quest _quest;
        [SerializeField]
        private TextMeshProUGUI _questTitle;
        [SerializeField]
        private Button _button;

        public static event Action<Quest> QuestSelectorButtonClicked;

        public void OnButtonClicked()
        {
            QuestSelectorButtonClicked?.Invoke(_quest);
        }

        public void UpdateView(IGameDataProvider<Quest> dataProvider)
        {
            _quest = dataProvider.Data;
            _questTitle.text = _quest.Info.Name;
            _button.interactable = !QuestJournal.Instance.IsQuestCompleted(_quest.ID);
        }
    }
}
