using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SunsetSystems.Inventory.UI
{
    public class ContainerGUI : MonoBehaviour
    {
        [SerializeField]
        private ContainerEntry _containerEntryPrefab;
        [SerializeField]
        private TextMeshProUGUI _title;
        [SerializeField]
        private GameObject _contentParent;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void OpenContainerGUI(ItemStorage container, string title)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(container.transform.position);
            this.transform.position = screenPoint;
            _title.text = title;
            _contentParent.transform.DestroyChildren();
            foreach (InventoryEntry entry in container.Contents)
            {
                ContainerEntry guiElement = Instantiate(_containerEntryPrefab, _contentParent.transform);
                guiElement.SetEntryContent(entry, container);
            }
            gameObject.SetActive(true);
        }
    }
}
