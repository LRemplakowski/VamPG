using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterPortraits
{
    public class PortraitIcon : MonoBehaviour
    {
        [SerializeField]
        private Image image;

        private void Start()
        {
            if (image == null)
                image = GetComponentInChildren<Image>();
        }

        internal void SetIcon(Sprite sprite)
        {
            image.sprite = sprite;
        }
    }
}
