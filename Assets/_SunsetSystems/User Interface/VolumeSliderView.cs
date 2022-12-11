using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SunsetSystems.UI
{
    public class VolumeSliderView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _valueDisplayText;
        [SerializeField]
        private string _volumeTypePrefix;

        public void UpdateView(float value)
        {
            if (value > 0)
                _valueDisplayText.text = $"{_volumeTypePrefix} volume: {Mathf.RoundToInt((value * 100))}%";
            else
                _valueDisplayText.text = $"{_volumeTypePrefix} volume: OFF";
        }
    }
}
