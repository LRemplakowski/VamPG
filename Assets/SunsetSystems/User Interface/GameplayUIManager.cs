using SunsetSystems.Entities;
using SunsetSystems.Inventory.UI;
using SunsetSystems.UI.Pause;
using SunsetSystems.Utils;
using System;
using UnityEngine;

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
        private HoverNameplate HoverNameplate { get; set; }

        public void HandleNameplateHover(INameplateReciever nameplateReciever)
        {
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
