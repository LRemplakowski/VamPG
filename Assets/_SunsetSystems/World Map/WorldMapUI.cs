using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.WorldMap
{
    public class WorldMapUI : SerializedMonoBehaviour
    {
        [Title("Travel Buttons")]
        [SerializeField, Required]
        private Button _enterCurrentArea;
        [SerializeField]
        private Vector2 _enterAreaButtonOffset;
        [SerializeField, Required]
        private Button _travelToArea;
        [SerializeField]
        private Vector2 _travelToAreaButtonOffset;

        [Title("References")]
        [SerializeField, Required]
        private IWorldMapManager _worldMapManager;
        [SerializeField]
        private Transform _mapTokenParent;
        [SerializeField, Required]
        private CanvasGroup _mapCanvasGroup;
        [SerializeField, Required]
        private AreaDescription _areaDescriptionWindow;
        [SerializeField, Required]
        private IWorldMapConfirmationWindow _areaTravelConfirmatonScreen;
        [SerializeField, Required]
        private IWorldMapConfirmationWindow _areaEnterConfirmationScreen;

        [Title("Data")]
        [SerializeField]
        private List<IWorldMapToken> _mapTokens = new();

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private IWorldMapData _selectedMap;
        [ShowInInspector, ReadOnly]
        private IWorldMapToken _selectedToken;
        [ShowInInspector, ReadOnly]
        private IWorldMapToken _currentAreaToken;
        [ShowInInspector, ReadOnly]
        private readonly Dictionary<string, IWorldMapToken> _idTokenDictionary = new();

        [Title("Editor Utility")]
        [SerializeField]
        private bool _autoCacheTokensInParent = true;

        private bool _lockCurrentToken = false;

        private void OnValidate()
        {
            EnsureTokenParent();
            if (_autoCacheTokensInParent)
                CacheMapTokens();
            else
                _mapTokens = _mapTokens.Distinct().ToList();
        }

        [Button]
        private void CacheMapTokens()
        {
            _mapTokens = _mapTokenParent.GetComponentsInChildren<IWorldMapToken>(true).ToList();
        }

        private void EnsureTokenParent() => _mapTokenParent = _mapTokenParent != null ? _mapTokenParent : transform;

        private void Awake()
        {
            EnsureTokenParent();
        }

        private void Start()
        {
            InitialTokenSetup();
            ShowUnlockedMapTokens();
        }

        private void Update()
        {
            UpdateTravelToAreaButton();
            UpdateEnterCurrentAreaButton();
        }

        private void InitialTokenSetup()
        {
            foreach (var token in _mapTokens)
            {
                _idTokenDictionary[token.GetData().DatabaseID] = token;
                token.InjectTokenManager(this);
                token.SetVisible(false);
            }
            var currentMap = _worldMapManager.GetCurrentMap();
            if (currentMap != null)
                _idTokenDictionary.TryGetValue(currentMap.DatabaseID, out _currentAreaToken);
        }

        private void UpdateTravelToAreaButton()
        {
            bool showTravelButton = _selectedMap != null && _worldMapManager.IsCurrentMap(_selectedMap) == false;
            _travelToArea.gameObject.SetActive(showTravelButton);
            if (showTravelButton)
                MoveButtonToTokenPosition(_travelToArea, _selectedToken, _travelToAreaButtonOffset);
        }

        private void UpdateEnterCurrentAreaButton()
        {
            bool showEnterButton = _worldMapManager.GetCurrentMap() != null;
            _enterCurrentArea.gameObject.SetActive(showEnterButton);
            if (showEnterButton)
                MoveButtonToTokenPosition(_enterCurrentArea, _currentAreaToken, _enterAreaButtonOffset);
        }

        private void ShowUnlockedMapTokens()
        {
            var unlockedMaps = _worldMapManager.GetUnlockedMaps();
            foreach (IWorldMapData map in unlockedMaps)
            {
                if (_idTokenDictionary.TryGetValue(map.DatabaseID, out IWorldMapToken token))
                {
                    token.SetVisible(true);
                    token.SetUnlocked(true);
                }
            }
        }

        public void ShowAreaDescription(IWorldMapData tokenData)
        {
            _selectedMap = tokenData;
            _idTokenDictionary.TryGetValue(tokenData.DatabaseID, out _selectedToken);
            _ = _areaDescriptionWindow.ShowDescription(tokenData, false);
        }

        public void HideAreaDescription()
        {
            _selectedMap = null;
            _ = _areaDescriptionWindow.HideDescription();
        }

        public void ToogleTravelConfirmationPopup(bool show)
        {
            if (show)
                _areaTravelConfirmatonScreen.ShowConfirmationWindow(_selectedMap);
            else
                _areaTravelConfirmatonScreen.HideConfirmationWindow();
        }

        public void ToggleEnterAreaConfirmationPopup(bool show)
        {
            if (show)
            {
                _areaEnterConfirmationScreen.ShowConfirmationWindow(_worldMapManager.GetCurrentMap());
            }
            else
            {
                _areaEnterConfirmationScreen.HideConfirmationWindow();
            }
        }

        public void ConfirmTravelToSelectedArea()
        {
            _worldMapManager.TravelToMap(_selectedMap);
            _idTokenDictionary.TryGetValue(_selectedMap.DatabaseID, out _currentAreaToken);
        }

        private void MoveButtonToTokenPosition(Button button, IWorldMapToken token, Vector2 buttonOffset)
        {
            if (token == null || button == null)
                return;
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, token.GetTokenPosition());
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, screenPoint, null, out var tokenCanvasPosition);
            button.transform.localPosition = tokenCanvasPosition + buttonOffset;
        }

        public void HandleTokenHoveredOver(bool hovered, IWorldMapData tokenData)
        {
            if (_lockCurrentToken)
                return;
            if (hovered)
                ShowAreaDescription(tokenData);
            else
                HideAreaDescription();
        }

        public void LockTokenDescription(bool locked, IWorldMapData tokenData, IWorldMapToken sourceToken)
        {
            if (locked == false)
            {
                HideAreaDescription();
                SetLocked(false);
                SetSelectedToken(null);
                return;
            }
            if (IsLocked() && IsCurrentSelectedToken(tokenData) is false)
            {
                ShowAreaDescription(tokenData);
                SetSelectedToken(sourceToken);
                return;
            }
            ShowAreaDescription(tokenData);
            SetSelectedToken(sourceToken);
            SetLocked(true);
        }

        private bool IsLocked()
        {
            return _lockCurrentToken;
        }

        private void SetLocked(bool locked)
        {
            _lockCurrentToken = locked;
        }

        private void SetSelectedToken(IWorldMapToken token)
        {
            _selectedToken = token;
        }

        private bool IsCurrentSelectedToken(IWorldMapData newToken)
        {
            try
            {
                return newToken.DatabaseID == _selectedMap.DatabaseID;
            }
            catch (NullReferenceException)
            {
                Debug.LogError("WorlMapUI >>> IsCurrentSelectedToken(IWorldMapData) >>> Recieved null token!");
                return false;
            }
        }

        internal void ConfirmEntryToCurrentArea()
        {
            throw new NotImplementedException();
        }
    }
}
