using Entities.Characters;
using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Management;
using UnityEngine;
using System;

namespace SunsetSystems.Journal
{
    public class JournalManager : Manager
    {
        [SerializeField]
        private CreatureData _playerCharacterData;
        public CreatureData PlayerCharacterData { get => _playerCharacterData; }
        [SerializeField]
        private CreatureData[] _recruitedCompanions = new CreatureData[0];
        public CreatureData[] RecruitedCompanions { get => _recruitedCompanions; }
        [SerializeField]
        private CreatureData[] _activeCompanions = new CreatureData[0];
        public CreatureData[] ActiveCompanions { get => _activeCompanions; }
        [SerializeField]
        private PlayerCharacterBackground playerBackground;

        private void OnEnable()
        {
            MainCharacter.onMainCharacterInitialized += Initialize;
        }

        private void OnDisable()
        {
            MainCharacter.onMainCharacterInitialized -= Initialize;
        }

        private void Initialize()
        {
            _playerCharacterData = FindObjectOfType<MainCharacter>().GetComponent<CreatureData>();
            
        }
    }
}
