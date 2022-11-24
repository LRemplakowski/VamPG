using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.MainMenu.UI
{
    [RequireComponent(typeof(Button))]
    public class StatsAssignmentButton : MainMenuNavigationButton
    {
        [SerializeField]
        private UnassignedDots unassignedAttributes, unassignedSkills;
        [SerializeField]
        private Button button; 

        protected void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();
        }

        protected override void Start()
        {
            base.Start();
            if (unassignedAttributes != null && unassignedSkills != null)
                button.interactable = !(unassignedAttributes.HasAvailableDotGroups() || unassignedSkills.HasAvailableDotGroups());
        }

        private void Update()
        {
            if (unassignedAttributes != null && unassignedSkills != null)
                button.interactable = !(unassignedAttributes.HasAvailableDotGroups() || unassignedSkills.HasAvailableDotGroups());
        }
    }
}
