using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SunsetSystems.Inventory.UI
{
    public class ContainerGUI : UIWindow
    {
        [SerializeField]
        private ContainerEntry _containerEntryPrefab;
        [SerializeField]
        private TextMeshProUGUI _title;
        [SerializeField]
        private GameObject _contentParent;
        private readonly List<ContainerEntry> _entries = new();

        public delegate void ContainerOpenedEvent();
        public static event ContainerOpenedEvent OnContainerOpened;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void OpenContainerGUI(ItemStorage container, string title)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(container.transform.position);
            transform.position = screenPoint;
            _title.text = title;
            ClearEntries();
            foreach (InventoryEntry entry in container.Contents)
            {
                ContainerEntry guiElement = Instantiate(_containerEntryPrefab, _contentParent.transform);
                guiElement.SetEntryContent(entry, container);
                _entries.Add(guiElement);
            }
            OnContainerOpened?.Invoke();
        }

        private void ClearEntries()
        {
            _entries.ForEach(e => Destroy(e.gameObject));
            _entries.Clear();
        }
    }
}
