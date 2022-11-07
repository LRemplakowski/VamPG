using NaughtyAttributes;
using SunsetSystems.UI.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Journal.UI
{
    public class QuestListing : MonoBehaviour, IUserInterfaceUpdateReciever<Quest, QuestContainer>
    {
        [SerializeField, Required]
        private QuestContainer _questContainerPrefab;
        [SerializeField]
        private GameObject _sectionTitle;

        public Transform ViewParent => transform;

        public List<IUserInterfaceView<Quest, QuestContainer>> ViewPool { get; } = new();

        public QuestContainer ViewPrefab => _questContainerPrefab;

        public void ListQuests(IList<Quest> quests)
        {
            if (quests == null || quests.Count() <= 0)
            {
                Debug.LogWarning("Quest listing recieved an empty or null collection!");
                DisableViews();
                return;
            }
            List<IGameDataProvider<Quest>> data = new();
            data.AddRange(quests);
            (this as IUserInterfaceUpdateReciever<Quest, QuestContainer>).UpdateViews(data);
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
