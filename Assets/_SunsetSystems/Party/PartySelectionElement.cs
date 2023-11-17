using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SunsetSystems.Entities.Characters;

namespace SunsetSystems.Party.UI
{
    public class PartySelectionElement : MonoBehaviour
    {
        [SerializeField]
        private Image image;
        [SerializeField]
        private TextMeshProUGUI text;

        // Start is called before the first frame update
        void Start()
        {
            if (image == null)
                image = GetComponentInChildren<Image>();
            if (text == null)
                text = GetComponentInChildren<TextMeshProUGUI>();
        }
        
        internal void Initialize(CreatureUIData data)
        {
            image.sprite = data.portrait;
            text.text = data.name;
        }
    }
}
