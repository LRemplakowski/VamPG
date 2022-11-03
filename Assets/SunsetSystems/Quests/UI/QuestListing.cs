using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Journal.UI
{
    public class QuestListing : MonoBehaviour
    {
        [SerializeField, Required]
        private QuestContainer _questContainerPrefab;
        [SerializeField]
        private GameObject _sectionTitle;

        private readonly List<QuestContainer> _containerPool = new();

        public void ListQuests(IList<Quest> quests, QuestView view)
        {
            if (quests == null || quests.Count() <= 0)
            {
                Debug.LogWarning("Quest listing recieved an empty or null collection!");
                return;
            }
            gameObject.SetActive(true);
            for (int i = 0; i < quests.Count(); i++)
            {
                Quest quest = quests[i];
                if (quest == null)
                    continue;
                QuestContainer container;
                if (_containerPool.Count > i)
                {
                    container = _containerPool[i];
                }
                else
                {
                    container = Instantiate(_questContainerPrefab, transform);
                    _containerPool.Add(container);
                }
                container.Initialize(quest, view);
                container.gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            _containerPool.ForEach(c => c.gameObject.SetActive(false));
        }
    }
}
