using SunsetSystems.Utils;
using UnityEngine;

namespace UI.CharacterPortraits
{
    public class PortraitController : MonoBehaviour
    {
        [SerializeField]
        private PortraitIcon portraitIcon;

        private void Start()
        {
            if (portraitIcon == null)
                portraitIcon = GetComponentInChildren<PortraitIcon>();
        }

        public void InitPotrait(Sprite portrait)
        {
            portraitIcon.SetIcon(portrait);
        }    
    }
}
