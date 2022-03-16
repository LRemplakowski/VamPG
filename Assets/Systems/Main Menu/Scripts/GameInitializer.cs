using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Characters;
using Transitions.Data;
using Transitions.Manager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Utils.ResourceLoader;

namespace SunsetSystems.GameData
{
    public class GameInitializer : MonoBehaviour
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
        private TransitionManager transitionManager;
        [SerializeField]
        private GameObject mainMenuParent;
        [SerializeField]
        private GameObject gameplayUiParent;

        private void Reset()
        {
            if (!stats)
                stats = ScriptableObject.CreateInstance<CharacterStats>();
        }

        private void Start()
        {
            if (!stats)
                stats = ScriptableObject.CreateInstance<CharacterStats>();
            if (!transitionManager)
                transitionManager = FindObjectOfType<TransitionManager>();
            if (!mainMenuParent)
                mainMenuParent = GameObject.FindGameObjectWithTag(MAIN_MENU);
            if (!gameplayUiParent)
                gameplayUiParent = GameObject.FindGameObjectWithTag(GAMEPLAY_UI);
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

        public void InitializeGame()
        {
            CreatureAsset mainCharacterAsset = CreatureAsset.CopyInstance(GetMatchingCreatureAsset());
            mainCharacterAsset.CreatureName = characterName;
            mainCharacterAsset.StatsAsset = stats;
            GameData journal = FindObjectOfType<GameData>();
            journal.MainCharacterAsset = mainCharacterAsset;
            TransitionData data = new NameTransition(startSceneName, startingEntryPointTag);
            transitionManager.PerformTransition(data);
            SubscribeOnFadedOutMethods();
        }

        private void SubscribeOnFadedOutMethods()
        {
            TransitionAnimator.OnFadedOut += EnableGamplayUI;
            TransitionAnimator.OnFadedOut += DisableMainMenuUI;
            TransitionAnimator.OnFadedOut += SwitchInputActionMap;
        }

        private void SwitchInputActionMap()
        {
            InputManager.ToggleActionMap(InputMap.Player);
        }

        private void EnableGamplayUI()
        {
            gameplayUiParent.SetActive(true);
            TransitionAnimator.OnFadedOut -= EnableGamplayUI;
        }

        private void DisableMainMenuUI()
        {
            mainMenuParent.SetActive(false);
            TransitionAnimator.OnFadedOut -= DisableMainMenuUI;
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
