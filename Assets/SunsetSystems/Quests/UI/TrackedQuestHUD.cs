using Apex;
using SunsetSystems.UI.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal.UI
{
    public class TrackedQuestHUD : MonoBehaviour, IUserInterfaceUpdateReciever<Quest, TrackedQuestView>
    {
        [SerializeField]
        private TrackedQuestView _trackedQuestViewPrefab;

        public List<IUserInterfaceView<Quest, TrackedQuestView>> ViewPool { get; } = new();
        public TrackedQuestView ViewPrefab => _trackedQuestViewPrefab;
        public Transform ViewParent => transform;

        private void OnEnable()
        {
            QuestJournal.OnTrackedQuestsChanged += UpdateTrackedQuests;
        }

        private void OnDisable()
        {
            QuestJournal.OnTrackedQuestsChanged -= UpdateTrackedQuests;
        }

        private void Start()
        {
            ViewPool.AddRange(GetComponentsInChildren<TrackedQuestView>());
            DisableViews();
        }

        private void UpdateTrackedQuests(List<Quest> quests)
        {
            if (quests == null)
                return;
            List<IGameDataProvider<Quest>> data = new();
            data.AddRange(quests);
            (this as IUserInterfaceUpdateReciever<Quest, TrackedQuestView>).UpdateViews(data);
        }

        public void DisableViews()
        {
            ViewPool.ForEach(view => (view as MonoBehaviour).gameObject.SetActive(false));
        }
    }
}
