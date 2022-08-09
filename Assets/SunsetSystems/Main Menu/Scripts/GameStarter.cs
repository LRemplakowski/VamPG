using Entities.Characters;
using SunsetSystems.Loading;
using UnityEngine;
using SunsetSystems.Resources;
using SunsetSystems.UI;
using SunsetSystems.Utils;

namespace SunsetSystems.Data
{
    public class GameStarter : Singleton<GameStarter>
    {
        private const string MAIN_MENU = "MainMenuParent";

        [SerializeField, ReadOnly]
        private PlayerCharacterBackground _selectedBackground;
        [SerializeField, ReadOnly]
        private BodyType _selectedBodyType;
        [SerializeField, ReadOnly]
        private string _characterName = "Alex";
        [SerializeField]
        private CharacterStats _stats;
        [SerializeField]
        private string _startSceneName;
        [SerializeField]
        private string _initialEntryPointTag;
        [SerializeField]
        private string _initialBoundingBoxTag;
        [SerializeField]
        private SceneLoader _sceneLoader;
        [SerializeField]
        private GameObject _mainMenuParent;

        private void Reset()
        {
            if (!_stats)
                _stats = ScriptableObject.CreateInstance<CharacterStats>();
        }

        private void Start()
        {
            if (!_stats)
                _stats = ScriptableObject.CreateInstance<CharacterStats>();
            if (!_sceneLoader)
                _sceneLoader = FindObjectOfType<SceneLoader>();
            if (!_mainMenuParent)
                _mainMenuParent = GameObject.FindGameObjectWithTag(MAIN_MENU);
        }

        public void SelectBackground(PlayerCharacterBackground selectedBackground)
        {
            this._selectedBackground = selectedBackground;
        }

        public void SelectBodyType(BodyType selectedBodyType)
        {
            this._selectedBodyType = selectedBodyType;
        }

        public void SetAttribueValue(AttributeType attribute, int value)
        {
            _stats.GetAttribute(attribute).SetValue(value);
        }

        public void SetSkillValue(SkillType skill, int value)
        {
            _stats.GetSkill(skill).SetValue(value);
        }

        public void SetCharacterName(string characterName)
        {
            this._characterName = characterName;
        }

        public async void InitializeGame()
        {
            Start();
            CreatureAsset mainCharacterAsset = CreatureAsset.CopyInstance(GetMatchingCreatureAsset());
            mainCharacterAsset.CreatureName = _characterName;
            mainCharacterAsset.StatsAsset = _stats;
            GameRuntimeData journal = FindObjectOfType<GameRuntimeData>();
            journal.MainCharacterAsset = mainCharacterAsset;
            SceneLoadingData data = new NameLoadingData(_startSceneName, _initialEntryPointTag, _initialBoundingBoxTag, DisableMainMenu);
            await _sceneLoader.LoadGameScene(data);
        }

        public async void InitializeGameDebug()
        {
            Start();
            CreatureAsset debugAsset = ResourceLoader.GetDefaultCreatureAsset();
            GameRuntimeData journal = FindObjectOfType<GameRuntimeData>();
            journal.MainCharacterAsset = debugAsset;
            SceneLoadingData data = new NameLoadingData(_startSceneName, _initialEntryPointTag, _initialBoundingBoxTag, DisableMainMenu);
            await _sceneLoader.LoadGameScene(data);
        }

        public void DisableMainMenu()
        {
            _mainMenuParent.SetActive(false);
        }

        private CreatureAsset GetMatchingCreatureAsset()
        {
            return _selectedBodyType switch
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
}
