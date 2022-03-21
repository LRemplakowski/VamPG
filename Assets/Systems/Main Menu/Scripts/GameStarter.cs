using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Characters;
using Transitions.Data;
using Transitions.Manager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Utils.Resources;
using Utils.Scenes;

namespace SunsetSystems.GameData
{
    public class GameStarter : MonoBehaviour
    {
        private const string MAIN_MENU = "MainMenuParent";
        private const string GAMEPLAY_UI = "GameParent";

        [SerializeField, ReadOnly]
        private PlayerCharacterBackground selectedBackground;
        [SerializeField, ReadOnly]
        private BodyType selectedBodyType;
        [SerializeField, ReadOnly]
        private string characterName = "Alex";
        [SerializeField]
        private CharacterStats stats;
        [SerializeField]
        private string startSceneName;
        [SerializeField]
        private string startingEntryPointTag;
        [SerializeField]
        private SceneLoader sceneLoader;
        [SerializeField]
        private GameObject mainMenuParent;
        [SerializeField]
        private GameObject gameplayUiParent;
        [SerializeField]
        private FadeScreenAnimator fadeScreenAnimator;

        private void Reset()
        {
            if (!stats)
                stats = ScriptableObject.CreateInstance<CharacterStats>();
        }

        private void Start()
        {
            if (!stats)
                stats = ScriptableObject.CreateInstance<CharacterStats>();
            if (!sceneLoader)
                sceneLoader = FindObjectOfType<SceneLoader>();
            if (!mainMenuParent)
                mainMenuParent = GameObject.FindGameObjectWithTag(MAIN_MENU);
            if (!gameplayUiParent)
                gameplayUiParent = GameObject.FindGameObjectWithTag(GAMEPLAY_UI);
            if (!fadeScreenAnimator)
                fadeScreenAnimator = FindObjectOfType<FadeScreenAnimator>(true);
        }

        public void SelectBackground(PlayerCharacterBackground selectedBackground)
        {
            this.selectedBackground = selectedBackground;
        }

        public void SelectBodyType(BodyType selectedBodyType)
        {
            this.selectedBodyType = selectedBodyType;
        }

        public void SetAttribueValue(AttributeType attribute, int value)
        {
            stats.GetAttribute(attribute).SetValue(value);
        }

        public void SetSkillValue(SkillType skill, int value)
        {
            stats.GetSkill(skill).SetValue(value);
        }

        public void SetCharacterName(string characterName)
        {
            this.characterName = characterName;
        }

        public async void InitializeGame()
        {
            CreatureAsset mainCharacterAsset = CreatureAsset.CopyInstance(GetMatchingCreatureAsset());
            mainCharacterAsset.CreatureName = characterName;
            mainCharacterAsset.StatsAsset = stats;
            GameData journal = FindObjectOfType<GameData>();
            journal.MainCharacterAsset = mainCharacterAsset;
            SceneLoadingData data = new NameLoadingData(startSceneName, startingEntryPointTag);
            await fadeScreenAnimator.FadeOut(.5f);
            SwitchUIAndInputToGameplayMode();
            await fadeScreenAnimator.FadeIn(.5f);
            _ = sceneLoader.LoadGameScene(data);
            StateManager.SetCurrentState(GameState.Exploration);
        }

        public void SwitchUIAndInputToGameplayMode()
        {
            EnableGamplayUI();
            DisableMainMenuUI();
            SwitchInputActionMap();
        }

        private void SwitchInputActionMap()
        {
            InputManager.ToggleActionMap(InputMap.Player);
        }

        private void EnableGamplayUI()
        {
            gameplayUiParent.SetActive(true);
        }

        private void DisableMainMenuUI()
        {
            mainMenuParent.SetActive(false);
        }

        private CreatureAsset GetMatchingCreatureAsset()
        {
            return selectedBodyType switch
            {
                BodyType.M => selectedBackground switch
                {
                    PlayerCharacterBackground.Agent => ResourceLoader.GetMaleAgentAsset(),
                    PlayerCharacterBackground.Convict => ResourceLoader.GetMaleConvictAsset(),
                    PlayerCharacterBackground.Journalist => ResourceLoader.GetMaleJournalistAsset(),
                    _ => ResourceLoader.GetDefaultCreatureAsset(),
                },
                BodyType.F => selectedBackground switch
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
