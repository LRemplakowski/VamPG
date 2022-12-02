using NaughtyAttributes;
using SunsetSystems.UI.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Journal.UI
{
    public class QuestListing : MonoBehaviour, IUserInterfaceUpdateReciever<Quest>
    {
        [SerializeField, Required]
        private QuestContainer _questContainerPrefab;
        [SerializeField]
        private GameObject _sectionTitle;

        public Transform ViewParent => transform;

        public List<QuestContainer> ViewPool { get; } = new();

        public QuestContainer ViewPrefab => _questContainerPrefab;

        public void ListQuests(List<Quest> quests)
        {
            UpdateViews(new List<IGameDataProvider<Quest>>(quests));
        }

        public void UpdateViews(List<IGameDataProvider<Quest>> data)
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
                IGameDataProvider<Quest> dataProvider = data[i];
                if (dataProvider == null)
                {
                    Debug.LogError("Null DataProvider while creating view!");
                    continue;
                }

                QuestContainer view;
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

        private void OnDisable()
        {
            DisableViews();
        }

        public void DisableViews()
        {
            gameObject.SetActive(false);
            ViewPool.ForEach(c => (c as MonoBehaviour).gameObject.SetActive(false));
        }
    }
}
