using System.Collections;
using UnityEngine;
using Utils.Singleton;

namespace InsaneSystems.RTSSelection
{
    public class SelectionEffect : MonoBehaviour
    {
        [SerializeField]
        private GameObject highlight;

        bool isSelected = false;

        private void OnEnable()
        {
            ISelectable selectable = GetComponent<ISelectable>();
            selectable.OnSelected += OnSelected;
            selectable.OnUnselected += OnUnselected;
        }

        private void OnDisable()
        {
            Selectable selectable = GetComponent<Selectable>();
            selectable.OnSelected -= OnSelected;
            selectable.OnUnselected -= OnUnselected;
        }

        private void Start()
        {
            if (highlight == null)
                highlight = GetComponentInChildren<Highlight>(true).gameObject;
        }

        void OnSelected() => SwitchSelectionHighlight();

        private void SwitchSelectionHighlight()
        {
            Debug.Log("SwitchingSelectionHighlight");
            isSelected = !isSelected;
            if (highlight != null)
                highlight.SetActive(isSelected);
        }

        void OnUnselected() => SwitchSelectionHighlight();
    }
}