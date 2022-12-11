using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SunsetSystems.UI
{
    public class TypewriterSliderView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _valueDisplayText;

        public void UpdateView(float value)
        {
            if (value > 0)
                _valueDisplayText.text = $"Typewriter Speed: {Mathf.RoundToInt(value)} letters per second";
            else
                _valueDisplayText.text = $"Typewriter Speed: Disabled";
        }
    }
}
