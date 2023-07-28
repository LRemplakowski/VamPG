using SunsetSystems.Entities.Characters;
using SunsetSystems.Persistence;
using UnityEngine;
using SunsetSystems.Resources;
using SunsetSystems.Utils;
using Sirenix.OdinInspector;
using SunsetSystems.Party;
using System.Collections.Generic;
using UltEvents;

namespace SunsetSystems.Data
{
    public class GameStarter : Singleton<GameStarter>
    {
        private const string MAIN_MENU = "MainMenuParent";

        [SerializeField]
        private CreatureData _playerCharacterData;
        [SerializeField]
        private PlayerCharacterBackground _selectedBackground;
        [SerializeField]
        private string _startSceneName;
        [SerializeField]
        private string _initialEntryPointTag;
        [SerializeField]
        private string _initialBoundingBoxTag;
        [SerializeField]
        private GameObject _mainMenuParent;
        [SerializeField]
        private List<GameObject> _objectsToReset;
        [field: SerializeField]
        public UltEvent OnGameStart { get; private set; }

        private void Start()
        {
            if (!_mainMenuParent)
                _mainMenuParent = GameObject.FindGameObjectWithTag(MAIN_MENU);
        }

        public void SelectBackground(PlayerCharacterBackground selectedBackground)
        {
            this._selectedBackground = selectedBackground;
        }

        public void SelectBodyType(BodyType selectedBodyType)
        {
            this._playerCharacterData.BodyType = selectedBodyType;
        }

        public void SetAttribueValue(AttributeType attribute, int value)
        {
            _playerCharacterData.Stats.Attributes.GetAttribute(attribute).SetValue(value);
        }

        public void SetSkillValue(SkillType skill, int value)
        {
            _playerCharacterData.Stats.Skills.GetSkill(skill).SetValue(value);
        }

        public void SetCharacterName(string characterName)
        {
            this._playerCharacterData.FirstName = characterName;
        }

        public async void InitializeGame()
        {
            Start();
            CreatureConfig mainCharacterAsset = GetMatchingCreatureAsset();
            PartyManager.RecruitMainCharacter(new(mainCharacterAsset));
            LevelLoadingData data = new NameLoadingData(_startSceneName, _initialEntryPointTag, _initialBoundingBoxTag, DisableMainMenu);
            //await _sceneLoader.LoadGameLevel(data);
        }

        public async void InitializeGameDebug()
        {
            Start();
            CreatureConfig debugAsset = ResourceLoader.GetDefaultCreatureAsset();
            PartyManager.RecruitMainCharacter(new(debugAsset));
            LevelLoadingData data = new NameLoadingData(_startSceneName, _initialEntryPointTag, _initialBoundingBoxTag, DisableMainMenu);
            //await _sceneLoader.LoadGameLevel(data);
        }

        public async void InitializeGameJam()
        {
            Start();
            SaveLoadManager.SetSaveID(new());
            CreatureConfig desiree = ResourceLoader.GetFemaleJournalistAsset();
            PartyManager.RecruitMainCharacter(new(desiree));
            LevelLoadingData data = new NameLoadingData(_startSceneName, _initialEntryPointTag, _initialBoundingBoxTag, DisableMainMenu);
            OnGameStart?.Invoke();
            SaveLoadManager.UpdateRuntimeDataCache();
            //await _sceneLoader.LoadGameLevel(data);
        }

        public void DisableMainMenu()
        {
            _mainMenuParent.SetActive(false);
        }

        private CreatureConfig GetMatchingCreatureAsset()
        {
            return _playerCharacterData.BodyType switch
            {
                BodyType.M => _selectedBackground switch
                {
                    PlayerCharacterBackground.Agent => ResourceLoader.GetMaleAgentAsset(),
                    PlayerCharacterBackground.Convict => ResourceLoader.GetMaleConvictAsset(),
                    PlayerCharacterBackground.Journalist => ResourceLoader.GetMaleJournalistAsset(),
                    _ => ResourceLoader.GetDefaultCreatureAsset(),
                },
                BodyType.F => _selectedBackground switch
                {
                    PlayerCharacterBackground.Agent => ResourceLoader.GetFemaleAgentAsset(),
                    PlayerCharacterBackground.Convict => ResourceLoader.GetFemaleConvictAsset(),
                    PlayerCharacterBackground.Journalist => ResourceLoader.GetFemaleJournalistAsset(),
                    _ => ResourceLoader.GetDefaultCreatureAsset(),
                },
                _ => ResourceLoader.GetDefaultCreatureAsset(),
            };
        }
    }

    public interface IResetable
    {
        void ResetOnGameStart();
    }
}
