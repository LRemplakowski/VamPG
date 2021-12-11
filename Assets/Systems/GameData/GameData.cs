using CleverCrow.Fluid.UniqueIds;
using Entities.Characters;
using SunsetSystems.SaveLoad;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using SunsetSystems.Management;
using SunsetSystems.Formation;

namespace SunsetSystems.GameData
{
    public class GameData : Manager, ISaveRuntimeData
    {
        [SerializeField]
        private CreatureData creaturePrefab;
        [ES3NonSerializable]
        private CreatureData mainCharData;
        [ES3NonSerializable]
        private List<CreatureData> companionsData = new List<CreatureData>();
        [SerializeField]
        private CreatureAsset _mainCharacterAsset;
        public CreatureAsset MainCharacterAsset { get => _mainCharacterAsset; set => _mainCharacterAsset = value; }
        [SerializeField]
        private List<CreatureAsset> _activeCompanionAssets = new List<CreatureAsset>();
        public List<CreatureAsset> ActiveCompanionAssets { get => _activeCompanionAssets; }
        [SerializeField]
        private List<CreatureAsset> _recruitedCompanionAssets = new List<CreatureAsset>();
        public List<CreatureAsset> RecruitedCompanionAssets { get => _recruitedCompanionAssets; }

        private bool isPartyInitialized = false;

        public void InitializeParty(Vector3 position)
        {
            List<Vector3> positions = FormationController.GetPositionsFromPoint(position);
            InitializeParty(positions);
        }

        public void InitializeParty(List<Vector3> positions)
        {
            if (_mainCharacterAsset != null && !isPartyInitialized)
            {
                mainCharData = InitializePartyMember(MainCharacterAsset, positions[0]);
                for (int i = 0; i < ActiveCompanionAssets.Count; i++)
                {
                    companionsData.Add(InitializePartyMember(ActiveCompanionAssets[i], positions[i + 1]));
                }

                isPartyInitialized = true;
            }
        }

        private CreatureData InitializePartyMember(CreatureAsset asset, Vector3 position)
        {
            CreatureData creature = Instantiate(creaturePrefab, position, Quaternion.identity);
            creature.SetData(asset);
            creature.CreateCreature();
            return creature;
        }

        public void SaveRuntimeData()
        {
            string id = typeof(GameData).Name;
            ES3.Save(id + "_mainCharData", _mainCharacterAsset);
            ES3.Save(id + "_mainCharacterStats", _mainCharacterAsset.StatsAsset);
            ES3.Save(id + "_activeCompanionsData", ActiveCompanionAssets);
            List<CharacterStats> activeCompanionsStats = new List<CharacterStats>();
            foreach (CreatureAsset companion in ActiveCompanionAssets)
                activeCompanionsStats.Add(companion.StatsAsset);
            ES3.Save(id + "_activeCompanionsStats", activeCompanionsStats);
            ES3.Save(id + "_recruitedCompanionsData", RecruitedCompanionAssets);
            List<CharacterStats> recruitedCompanionsStats = new List<CharacterStats>();
            foreach (CreatureAsset companion in RecruitedCompanionAssets)
                recruitedCompanionsStats.Add(companion.StatsAsset);
            ES3.Save(id + "_recruitedCompanionsStats", recruitedCompanionsStats);

            List<Vector3> partyPositions = new List<Vector3>();
            partyPositions.Add(mainCharData.transform.position);
            foreach (CreatureData companion in companionsData)
            {
                partyPositions.Add(companion.transform.position);
            }
            ES3.Save(id + "_activePartyPositions", partyPositions);
        }

        public void LoadRuntimeData()
        {
            string id = typeof(GameData).Name;
            MainCharacterAsset = ES3.Load<CreatureAsset>(id + "_mainCharData");
            _activeCompanionAssets = ES3.Load<List<CreatureAsset>>(id + "_activeCompanionsData");
            List<CharacterStats> activeStats = ES3.Load<List<CharacterStats>>(id + "_activeCompanionsStats");
            for (int i = 0; i < ActiveCompanionAssets.Count; i++)
            {
                ActiveCompanionAssets[i].StatsAsset = activeStats[i];
            }
            _recruitedCompanionAssets = ES3.Load<List<CreatureAsset>>(id + "_recruitedCompanionsData");
            List<CharacterStats> recruitedStats = ES3.Load<List<CharacterStats>>(id + "_recruitedCompanionsStats");
            for (int i = 0; i < RecruitedCompanionAssets.Count; i++)
            {
                RecruitedCompanionAssets[i].StatsAsset = recruitedStats[i];
            }
            List<Vector3> positions = ES3.Load<List<Vector3>>(id + "_activePartyPositions");
            InitializeParty(positions);
        }

        public GameDataContainer GetCurrentJournalData()
        {
            return new GameDataContainer.GameDataContainerBuilder()
                .SetMainCharacterAsset(MainCharacterAsset)
                .SetActiveCompanionAssets(ActiveCompanionAssets)
                .SetRecruitedCompanionAssets(RecruitedCompanionAssets)
                .Build();
        }

        public void InjectJournalData(GameDataContainer data)
        {
            _mainCharacterAsset = data.mainCharacterAsset;
            _activeCompanionAssets = new List<CreatureAsset>(data.ActiveCompanionAssets);
            _recruitedCompanionAssets = new List<CreatureAsset>(data.RecruitedCompanionAssets);
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
