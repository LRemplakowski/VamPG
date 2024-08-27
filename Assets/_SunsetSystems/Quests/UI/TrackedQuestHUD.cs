using SunsetSystems.UI.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal.UI
{
    public class TrackedQuestHUD : MonoBehaviour, IUserInterfaceUpdateReciever<Quest>
    {
        [SerializeField]
        private TrackedQuestView _trackedQuestViewPrefab;

        public List<TrackedQuestView> ViewPool { get; } = new();
        public TrackedQuestView ViewPrefab => _trackedQuestViewPrefab;
        public Transform ViewParent => transform;

        private void OnEnable()
        {
            QuestJournal.OnActiveQuestsChanged += UpdateTrackedQuests;
        }

        private void OnDisable()
        {
            QuestJournal.OnActiveQuestsChanged -= UpdateTrackedQuests;
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
            List<IUserInfertaceDataProvider<Quest>> data = new();
            data.AddRange(quests);
            UpdateViews(data);
        }

        public void UpdateViews(List<IUserInfertaceDataProvider<Quest>> data)
        {
            DisableViews();
            if (data == null || data.Count <= 0)
            {
                Debug.LogWarning("UI Update Reciever recieved an empty or null collection!");
                return;
            }
            ViewParent.gameObject.SetActive(true);
            for (int i = 0; i < data.Count; i++)
            {
                IUserInfertaceDataProvider<Quest> dataProvider = data[i];
                if (dataProvider == null)
                {
                    Debug.LogError("Null DataProvider while creating view!");
                    continue;
                }

                TrackedQuestView view;
                if (ViewPool.Count > i)
                {
                    Debug.Log("Getting view from pool!");
                    view = ViewPool[i];
                }
                else
                {
                    Debug.Log("Instantiating new view!");
                    view = Instantiate(ViewPrefab, ViewParent);
                    ViewPool.Add(view);
                }
                view.UpdateView(dataProvider);
                view.gameObject.SetActive(true);
            }
            gameObject.SetActive(true);
        }

        public void DisableViews()
        {
            ViewPool.ForEach(view => (view as MonoBehaviour).gameObject.SetActive(false));
        }
    }
}
