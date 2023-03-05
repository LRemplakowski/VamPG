using SunsetSystems.Entities;
using SunsetSystems.Inventory.UI;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.UI
{
    public interface IGameplayUI
    {
        public InGameUI InGameUI { get; }
        public PauseMenuUI PauseMenuUI { get; }
        public ContainerGUI ContainerGUI { get; }
        public HoverNameplate HoverNameplate { get; }
        public DialogueViewBase DialogueGUI { get; }
        public GameObject HelpOverlay { get; }

        void ShowNameplate(INameplateReciever nameplateReciever);

        void DisableNameplate();
    }
}
