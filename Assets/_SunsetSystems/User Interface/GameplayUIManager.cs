using SunsetSystems.Entities;
using SunsetSystems.Inventory.UI;
using SunsetSystems.UI.Pause;
using SunsetSystems.Utils;
using System;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.UI
{
    [RequireComponent(typeof(Tagger))]
    public class GameplayUIManager : MonoBehaviour
    {
        [field: SerializeField]
        public InGameUI InGameUI { get; private set; }
        [field: SerializeField]
        public PauseMenuUI PauseMenuUI { get; private set; }
        [field: SerializeField]
        public ContainerGUI ContainerGUI { get; private set; }
        [field: SerializeField]
        public HoverNameplate HoverNameplate { get; private set; }
        [field: SerializeField]
        public DialogueViewBase DialogueGUI { get; private set; }
        [field: SerializeField]
        public GameObject HelpOverlay { get; private set; }

        public void HandleNameplateHover(INameplateReciever nameplateReciever)
        {
            if (string.IsNullOrEmpty(nameplateReciever.NameplateText))
            {
                DisableNameplate();
                return;
            }
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(nameplateReciever.NameplateWorldPosition);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(InGameUI.transform as RectTransform, screenPoint, Camera.main, out Vector2 nameplatePosition);
            HoverNameplate.transform.position = screenPoint;
            HoverNameplate.SetNameplateText(nameplateReciever.NameplateText);
            HoverNameplate.gameObject.SetActive(true);
        }

        public void DisableNameplate()
        {
            HoverNameplate.gameObject.SetActive(false);
        }
    }
}
