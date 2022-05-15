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
        private GameObject _entriesParent;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void OpenContainerGUI(List<InventoryEntry> entries, Vector3 containerPosition)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(containerPosition);
            this.transform.position = screenPoint;
            _entriesParent.transform.DestroyChildren();
            foreach (InventoryEntry entry in entries)
            {
                ContainerEntry guiElement = Instantiate(_containerEntryPrefab, _entriesParent.transform);
                guiElement.Entry = entry;
            }
            gameObject.SetActive(true);
        }
    }
}
