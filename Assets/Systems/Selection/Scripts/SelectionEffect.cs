using UnityEngine;

namespace InsaneSystems.RTSSelection
{
    public class SelectionEffect : MonoBehaviour
    {
        [SerializeField]
        private GameObject highlight;

        bool isSelected = false;

        private void Reset()
        {
            if (highlight == null)
                highlight = Instantiate(Resources.Load("DEBUG/HoverHighlight"), this.transform) as GameObject;
        }

        private void Awake()
        {
            if (highlight == null)
                highlight = Instantiate(Resources.Load("DEBUG/HoverHighlight"), this.transform) as GameObject;
        }

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
            isSelected = !isSelected;
            if (highlight != null)
                highlight.SetActive(isSelected);
        }

        void OnUnselected() => SwitchSelectionHighlight();
    }
}