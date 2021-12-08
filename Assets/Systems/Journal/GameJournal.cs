using CleverCrow.Fluid.UniqueIds;
using Entities.Characters;
using SunsetSystems.SaveLoad;
using System.Collections.Generic;
using UnityEngine;
using Utils.Scenes;
using Utils.Singleton;

namespace SunsetSystems.Journal
{
    [RequireComponent(typeof(UniqueId))]
    public class GameJournal : ExposableMonobehaviour, ISaveRuntimeData, IMoveBetweenScenes
    {
        [SerializeField]
        private CreatureData creaturePrefab;
        [SerializeField, ReadOnly, ES3NonSerializable]
        private CreatureData _playerCharacterData;
        public CreatureData PlayerCharacterData 
        { 
            get => _playerCharacterData; 
        }
        [SerializeField, ReadOnly, ES3NonSerializable]
        private CreatureData[] _activeCompanions = new CreatureData[0];
        public CreatureData[] ActiveCompanions { get => _activeCompanions; }

        [SerializeField]
        private CreatureAsset _mainCharacterAsset;
        public CreatureAsset MainCharacterAsset { get => _mainCharacterAsset; set => _mainCharacterAsset = value; }
        [SerializeField]
        private List<CreatureAsset> _activeCompanionAssets = new List<CreatureAsset>();
        public List<CreatureAsset> ActiveCompanionAssets { get => _activeCompanionAssets; }
        [SerializeField]
        private List<CreatureAsset> _recruitedCompanionAssets = new List<CreatureAsset>();
        public List<CreatureAsset> RecruitedCompanionAssets { get => _recruitedCompanionAssets; }

        public void InitializeParty(Vector3 position)
        {             
            if (_mainCharacterAsset != null)
            {
                _playerCharacterData = Instantiate(creaturePrefab, position, Quaternion.identity);
                _playerCharacterData.SetData(MainCharacterAsset);
                _playerCharacterData.CreateCreature();
            }
        }

        public void SaveRuntimeData()
        {
            UniqueId unique = GetComponent<UniqueId>();
            ES3.Save(unique.Id + "_mainCharData", _mainCharacterAsset);
            ES3.Save(unique.Id + "_mainCharacterStats", _mainCharacterAsset.StatsAsset);
            ES3.Save(unique.Id + "_activeCompanionsData", ActiveCompanionAssets);
            List<CharacterStats> activeCompanionsStats = new List<CharacterStats>();
            foreach (CreatureAsset companion in ActiveCompanionAssets)
                activeCompanionsStats.Add(companion.StatsAsset);
            ES3.Save(unique.Id + "_activeCompanionsStats", activeCompanionsStats);
            ES3.Save(unique.Id + "_recruitedCompanionsData", RecruitedCompanionAssets);
            List<CharacterStats> recruitedCompanionsStats = new List<CharacterStats>();
            foreach (CreatureAsset companion in RecruitedCompanionAssets)
                recruitedCompanionsStats.Add(companion.StatsAsset);
            ES3.Save(unique.Id + "_recruitedCompanionsStats", recruitedCompanionsStats);
        }

        public void LoadRuntimeData()
        {
            UniqueId unique = GetComponent<UniqueId>();
            MainCharacterAsset = ES3.Load<CreatureAsset>(unique.Id + "_mainCharData");
            _activeCompanionAssets = ES3.Load<List<CreatureAsset>>(unique.Id + "_activeCompanionsData");
            List<CharacterStats> activeStats = ES3.Load<List<CharacterStats>>(unique.Id + "_activeCompanionsStats");
            for (int i = 0; i < ActiveCompanionAssets.Count; i++)
            {
                ActiveCompanionAssets[i].StatsAsset = activeStats[i];
            }
            _recruitedCompanionAssets = ES3.Load<List<CreatureAsset>>(unique.Id + "_recruitedCompanionsData");
            List<CharacterStats> recruitedStats = ES3.Load<List<CharacterStats>>(unique.Id + "_recruitedCompanionsStats");
            for (int i = 0; i < RecruitedCompanionAssets.Count; i++)
            {
                RecruitedCompanionAssets[i].StatsAsset = recruitedStats[i];
            }
        }

        public GameObject GetGameObject()
        {
            return this.gameObject;
        }
    }
}
