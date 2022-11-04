using SunsetSystems.UI.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Journal.UI
{
    public class TrackedQuestView : MonoBehaviour, IUserInterfaceView<Quest, TrackedQuestView>
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TextMeshProUGUI _title, _objective;

        public void UpdateView(IGameDataProvider<Quest> dataProvider)
        {
            _title.text = dataProvider.Data.QuestData.Name;
            _objective.text = default;
            if (QuestJournal.Instance.GetCurrentObjective(dataProvider.Data.ID, out Objective objective))
            {
                _objective.text = objective.Description;
            }
        }
    }
}
