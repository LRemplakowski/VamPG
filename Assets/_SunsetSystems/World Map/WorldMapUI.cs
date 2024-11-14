using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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

        [Title("Player Token")]
        [SerializeField, Required]
        private WorldMapPlayerToken _playerToken;

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
            {
                CacheMapTokens();
            }
            else
            {
                _mapTokens = _mapTokens.Distinct().ToList();
            }
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
            _playerToken.MoveToToken(_currentAreaToken, true);
            WorldMapManager.OnUnlockedMapsUpdated += OnMapsUpdated;
        }

        private void OnDestroy()
        {
            WorldMapManager.OnUnlockedMapsUpdated -= OnMapsUpdated;
        }

        private void LateUpdate()
        {
            UpdateTravelToAreaButton();
            UpdateEnterCurrentAreaButton();
        }

        private void InitialTokenSetup()
        {
            foreach (var token in _mapTokens)
            {
                var tokenMapData = token.GetData();
                if (tokenMapData != null)
                {
                    _idTokenDictionary[tokenMapData.DatabaseID] = token;
                }
                token.InjectTokenManager(this);
                token.SetVisible(false);
            }
            var currentMap = _worldMapManager.GetCurrentMap();
            if (currentMap != null)
            {
                _idTokenDictionary.TryGetValue(currentMap.DatabaseID, out _currentAreaToken);
            }
        }

        private void UpdateTravelToAreaButton()
        {
            var selectedMap = _worldMapManager.GetSelectedMap();
            bool showTravelButton = selectedMap != null && _worldMapManager.IsCurrentMap(selectedMap) == false;
            _travelToArea.gameObject.SetActive(showTravelButton);
            if (showTravelButton)
            {
                MoveButtonToTokenPosition(_travelToArea, _selectedToken, _travelToAreaButtonOffset);
            }
        }

        private void UpdateEnterCurrentAreaButton()
        {
            bool showEnterButton = _worldMapManager.GetCurrentMap() != null;
            _enterCurrentArea.gameObject.SetActive(showEnterButton);
            if (showEnterButton)
            {
                MoveButtonToTokenPosition(_enterCurrentArea, _currentAreaToken, _enterAreaButtonOffset);
            }
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

        private void OnMapsUpdated()
        {
            LockAllTokens();
            ShowUnlockedMapTokens();

            void LockAllTokens() => _idTokenDictionary.Values.ForEach(token => token.SetUnlocked(false));
        }

        public void ShowAreaDescription(IWorldMapData tokenData)
        {
            _idTokenDictionary.TryGetValue(tokenData.DatabaseID, out _selectedToken);
            _ = _areaDescriptionWindow.ShowDescription(tokenData, false);
        }

        public void HideAreaDescription()
        {
            _ = _areaDescriptionWindow.HideDescription();
        }

        public void ToogleTravelConfirmationPopup(bool show)
        {
            if (show)
            {
                _areaTravelConfirmatonScreen.ShowConfirmationWindow(_worldMapManager.GetSelectedMap());
            }
            else
            {
                _areaTravelConfirmatonScreen.HideConfirmationWindow();
            }
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
            _worldMapManager.TravelToSelectedMap();
            _idTokenDictionary.TryGetValue(_worldMapManager.GetSelectedMap().DatabaseID, out _currentAreaToken);
            UpdatePlayerTokenDestination();
        }

        private void MoveButtonToTokenPosition(Button button, IWorldMapToken token, Vector2 buttonOffset)
        {
            if (token == null || button == null)
                return;
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, token.GetTokenPosition());
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, screenPoint, null, out var tokenCanvasPosition);
            button.transform.localPosition = tokenCanvasPosition + buttonOffset;
        }

        private void UpdatePlayerTokenDestination()
        {
            _playerToken.MoveToToken(_selectedToken);
        }

        public void HandleTokenHoveredOver(bool hovered, IWorldMapData tokenData)
        {
            if (_lockCurrentToken)
                return;
            if (hovered)
            {
                ShowAreaDescription(tokenData);
            }
            else
            {
                HideAreaDescription();
            }
        }

        public void LockTokenDescription(bool locked, IWorldMapData tokenData, IWorldMapToken sourceToken)
        {
            if (locked == false)
            {
                HideAreaDescription();
                SetLocked(false);
                //SetSelectedToken(null);
                return;
            }
            if (IsLocked() && IsCurrentSelectedToken(sourceToken) is false)
            {
                _worldMapManager.SetSelectedMap(tokenData);
                ShowAreaDescription(tokenData);
                SetSelectedToken(sourceToken);
                return;
            }
            _worldMapManager.SetSelectedMap(tokenData);
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

        private bool IsCurrentSelectedToken(IWorldMapToken newToken)
        {
            try
            {
                return newToken.GetData().DatabaseID == _selectedToken.GetData().DatabaseID;
            }
            catch (NullReferenceException)
            {
                Debug.LogError("WorlMapUI >>> IsCurrentSelectedToken(IWorldMapData) >>> Recieved null token!");
                return false;
            }
        }

        public void ConfirmEntryToCurrentArea()
        {
            _worldMapManager.EnterCurrentMap();
        }
    }
}
