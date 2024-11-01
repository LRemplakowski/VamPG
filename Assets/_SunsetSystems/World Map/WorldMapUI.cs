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
        [SerializeField, Required]
        private Button _travelToArea;

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
        private WorldMapTravelConfirmation _areaConfirmatonScreen;

        [Title("Data")]
        [SerializeField]
        private List<IWorldMapToken> _mapTokens = new();

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private IWorldMapData _selectedMap;
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
            
        }

        private void InitialTokenSetup()
        {
            foreach (var token in _mapTokens)
            {
                _idTokenDictionary[token.GetData().DatabaseID] = token;
                token.InjectTokenManager(this);
                token.SetVisible(false);
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

        private void OnEnable()
        {
            var unlockedMaps = _worldMapManager.GetUnlockedMaps();
            _mapTokens.ForEach(token => token.SetVisible(false));
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
                _areaConfirmatonScreen.ShowConfirmationWindow();
            else
                _areaConfirmatonScreen.HideConfirmationWindow();
        }

        public void ConfirmTravelToSelectedArea()
        {
            _worldMapManager.TravelToMap(_selectedMap);
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

        public void LockTokenDescription(bool locked, IWorldMapData tokenData)
        {
            if (locked == false)
            {
                HideAreaDescription();
                SetLocked(false);
                return;
            }
            if (IsLocked() && IsCurrentSelectedToken(tokenData) is false)
            {
                ShowAreaDescription(tokenData);
                return;
            }
            ShowAreaDescription(tokenData);
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
    }
}
