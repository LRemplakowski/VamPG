using Entities.Characters;
using SunsetSystems.SaveLoad;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace SunsetSystems.GameData
{
    public class GameData : MonoBehaviour, ISaveRuntimeData
    {
        [ES3NonSerializable]
        private CreatureData _mainCharData;
        public CreatureData MainCharDataComponent { get => _mainCharData; set => _mainCharData = value; }
        [ES3NonSerializable]
        private List<CreatureData> _companionsData = new List<CreatureData>();
        public List<CreatureData> ActivePartyDataComponents { get => _companionsData; }
        [SerializeField]
        private CreatureAsset _mainCharacterAsset;
        public CreatureAsset MainCharacterAsset { get => _mainCharacterAsset; set => _mainCharacterAsset = value; }
        [SerializeField]
        private List<CreatureAsset> _activePartyAssets = new List<CreatureAsset>();
        public List<CreatureAsset> ActivePartyAssets { get => _activePartyAssets; }
        [SerializeField]
        private List<CreatureAsset> _inactivePartyAssets = new List<CreatureAsset>();
        public List<CreatureAsset> InactivePartyAssets { get => _inactivePartyAssets; }

        public void SaveRuntimeData()
        {
            string id = typeof(GameData).Name;
            SaveMainCharacter(id);
            SaveActiveCompanions(id);
            SaveActivePartyPositions(id);
            SaveRecruitedCompanions(id);
        }

        private void SaveActivePartyPositions(string id)
        {
            List<Vector3> partyPositions = new List<Vector3>();
            partyPositions.Add(_mainCharData.transform.position);
            foreach (CreatureData companion in _companionsData)
            {
                partyPositions.Add(companion.transform.position);
            }
            ES3.Save(id + "_activePartyPositions", partyPositions);
        }

        private void SaveRecruitedCompanions(string id)
        {
            ES3.Save(id + "_recruitedCompanionsData", InactivePartyAssets);
            List<CharacterStats> recruitedCompanionsStats = new List<CharacterStats>();
            foreach (CreatureAsset companion in InactivePartyAssets)
                recruitedCompanionsStats.Add(companion.StatsAsset);
            ES3.Save(id + "_recruitedCompanionsStats", recruitedCompanionsStats);
        }

        private void SaveActiveCompanions(string id)
        {
            ES3.Save(id + "_activeCompanionsData", ActivePartyAssets);
            List<CharacterStats> activeCompanionsStats = new List<CharacterStats>();
            foreach (CreatureAsset companion in ActivePartyAssets)
                activeCompanionsStats.Add(companion.StatsAsset);
            ES3.Save(id + "_activeCompanionsStats", activeCompanionsStats);
        }

        private void SaveMainCharacter(string id)
        {
            ES3.Save(id + "_mainCharData", _mainCharacterAsset);
            ES3.Save(id + "_mainCharacterStats", _mainCharacterAsset.StatsAsset);
        }

        public void LoadRuntimeData()
        {
            string id = typeof(GameData).Name;
            MainCharacterAsset = ES3.Load<CreatureAsset>(id + "_mainCharData");
            _activePartyAssets = ES3.Load<List<CreatureAsset>>(id + "_activeCompanionsData");
            List<CharacterStats> activeStats = ES3.Load<List<CharacterStats>>(id + "_activeCompanionsStats");
            for (int i = 0; i < ActivePartyAssets.Count; i++)
            {
                ActivePartyAssets[i].StatsAsset = activeStats[i];
            }
            _inactivePartyAssets = ES3.Load<List<CreatureAsset>>(id + "_recruitedCompanionsData");
            List<CharacterStats> recruitedStats = ES3.Load<List<CharacterStats>>(id + "_recruitedCompanionsStats");
            for (int i = 0; i < InactivePartyAssets.Count; i++)
            {
                InactivePartyAssets[i].StatsAsset = recruitedStats[i];
            }
            List<Vector3> positions = ES3.Load<List<Vector3>>(id + "_activePartyPositions");
        }

        public GameDataContainer GetCurrentJournalData()
        {
            return new GameDataContainer.GameDataContainerBuilder()
                .SetMainCharacterAsset(MainCharacterAsset)
                .SetActiveCompanionAssets(ActivePartyAssets)
                .SetRecruitedCompanionAssets(InactivePartyAssets)
                .Build();
        }

        public void InjectJournalData(GameDataContainer data)
        {
            _mainCharacterAsset = data.mainCharacterAsset;
            _activePartyAssets = new List<CreatureAsset>(data.ActiveCompanionAssets);
            _inactivePartyAssets = new List<CreatureAsset>(data.RecruitedCompanionAssets);
        }
    }

    public class GameDataContainer
    {
        public readonly CreatureAsset mainCharacterAsset;
        private readonly List<CreatureAsset> _activeCompanionAssets;
        public ReadOnlyCollection<CreatureAsset> ActiveCompanionAssets => _activeCompanionAssets.AsReadOnly();
        private readonly List<CreatureAsset> _recruitedCompanionAssets;
        public ReadOnlyCollection<CreatureAsset> RecruitedCompanionAssets => _recruitedCompanionAssets.AsReadOnly();

        private GameDataContainer(CreatureAsset mainCharacterAsset, List<CreatureAsset> activeCompanionAssets, List<CreatureAsset> recruitedCompanionAssets)
        {
            this.mainCharacterAsset = mainCharacterAsset;
            _activeCompanionAssets = activeCompanionAssets;
            _recruitedCompanionAssets = recruitedCompanionAssets;
        }

        public class GameDataContainerBuilder
        {
            private CreatureAsset mainCharacterAsset;
            private List<CreatureAsset> activeCompanionAssets;
            private List<CreatureAsset> recruitedCompanionAssets;

            public GameDataContainerBuilder SetMainCharacterAsset(CreatureAsset mainCharacterAsset)
            {
                this.mainCharacterAsset = mainCharacterAsset;
                return this;
            }

            public GameDataContainerBuilder SetActiveCompanionAssets(List<CreatureAsset> activeCompanionAssets)
            {
                this.activeCompanionAssets = activeCompanionAssets;
                return this;
            }

            public GameDataContainerBuilder SetRecruitedCompanionAssets(List<CreatureAsset> recruitedCompanionAssets)
            {
                this.recruitedCompanionAssets = recruitedCompanionAssets;
                return this;
            }

            public GameDataContainer Build()
            {
                GameDataContainer data = new GameDataContainer(mainCharacterAsset, activeCompanionAssets, recruitedCompanionAssets);
                return data;
            }
        }
    }
}
