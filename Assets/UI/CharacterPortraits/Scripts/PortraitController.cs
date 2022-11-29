using Entities.Characters.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.CharacterPortraits
{
    public class PortraitController : MonoBehaviour
    {
        [SerializeField]
        private PortraitIcon potraitIcon;

        private void Start()
        {
            if (potraitIcon == null)
                potraitIcon = GetComponentInChildren<PortraitIcon>();
        }

        internal void InitPotrait(CreatureUIData data)
        {
            potraitIcon.SetIcon(data.portrait);
        }    
    }
}
