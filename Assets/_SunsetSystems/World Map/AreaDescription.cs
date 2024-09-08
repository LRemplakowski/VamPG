using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Redcode.Awaiting;
using Sirenix.OdinInspector;
using SunsetSystems.WorldMap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems
{
    public class AreaDescription : SerializedMonoBehaviour
    {
        [Title("Config")]
        [SerializeField]
        private Transform _hiddenPosition;
        [SerializeField]
        private Transform _shownPosition;
        [SerializeField]
        private float _windowMoveTime = .5f;

        [Title("References")]
        [SerializeField, Required]
        private RectTransform _tweenTransform;
        [SerializeField, Required]
        private TextMeshProUGUI _areaTitle;
        [SerializeField, Required]
        private Image _areaPicture;
        [SerializeField, Required]
        private TextMeshProUGUI _travelTime;
        [SerializeField, Required]
        private TextMeshProUGUI _areaDescription;
        [SerializeField, Required]
        private GridLayoutGroup _activeQuests;

        private Tween _showHideTween;
        private bool _isVisible;

        private void OnValidate()
        {
            if (_tweenTransform == null)
                _tweenTransform = transform as RectTransform;
        }

        private void Start()
        {
            transform.position = _hiddenPosition.position;
        }

        [Button]
        public async Task ShowDescription(IWorldMapData mapData)
        {
            ForceCompletePreviousTween();
            if (_isVisible)
                await HideDescription();
            InjectMapData(mapData);
            _showHideTween = _tweenTransform.DOMove(_shownPosition.position, _windowMoveTime);
            _isVisible = true;
            await new WaitForSeconds(_windowMoveTime);
        }

        private void InjectMapData(IWorldMapData data)
        {
            if (data == null)
            {
                Debug.LogError("Tried to show Area Descritpion, but IWorldMapData is null!");
                return;
            }

            _areaTitle.text = data.GetAreaName();
            _areaDescription.text = data.GetDescription();
            _areaPicture.sprite = data.GetIcon();
        }

        private void ForceCompletePreviousTween()
        {
            if (_showHideTween != null)
            {
                _showHideTween.Complete();
                _showHideTween = null;
            }
        }

        [Button]
        public async Task HideDescription()
        {
            ForceCompletePreviousTween();
            _showHideTween = _tweenTransform.DOMove(_hiddenPosition.position, _windowMoveTime);
            _isVisible = false;
            await new WaitForSeconds(_windowMoveTime);
        }
    }
}
