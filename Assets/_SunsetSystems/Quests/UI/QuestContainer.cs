using SunsetSystems.UI.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        public void OnButtonClicked()
        {
            QuestSelectorButtonClicked?.Invoke(_quest);
        }

        public void UpdateView(IUserInfertaceDataProvider<Quest> dataProvider)
        {
            _quest = dataProvider.UIData;
            _questTitle.text = _quest.Name;
            _button.interactable = !QuestJournal.Instance.IsQuestCompleted(_quest.DatabaseID);
        }
    }
}
