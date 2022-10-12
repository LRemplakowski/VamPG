using CleverCrow.Fluid.UniqueIds;
using Entities.Characters;
using SunsetSystems.Loading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using SunsetSystems.Utils;

namespace SunsetSystems.Data
{
    public class GameRuntimeData : Singleton<GameRuntimeData>, ISaveRuntimeData
    {
        public void SaveRuntimeData()
        {
            Debug.Log("Saving runtime data");
            string id = GetComponent<UniqueId>().Id;
            //SaveMainCharacter(id);
            //SaveActiveCompanions(id);
            //SaveActivePartyPositions(id);
            //SaveRecruitedCompanions(id);
        }

        //private void SaveActivePartyPositions(string id)
        //{
        //    List<Vector3> partyPositions = new()
        //    {
        //        _mainCharacterData.transform.position
        //    };
        //    foreach (CreatureData companion in _activePartyData)
        //    {
        //        partyPositions.Add(companion.transform.position);
        //    }
        //    ES3.Save(id + "_activePartyPositions", partyPositions);
        //}

        //private void SaveRecruitedCompanions(string id)
        //{
        //    ES3.Save(id + "_recruitedCompanionsData", RecruitedMemberAssets);
        //    List<CharacterStats> recruitedCompanionsStats = new();
        //    foreach (CreatureConfig companion in RecruitedMemberAssets)
        //        recruitedCompanionsStats.Add(companion.StatsAsset);
        //    ES3.Save(id + "_recruitedCompanionsStats", recruitedCompanionsStats);
        //}

        //private void SaveActiveCompanions(string id)
        //{
        //    ES3.Save(id + "_activeCompanionsData", ActiveMemberAssets);
        //    List<CharacterStats> activeCompanionsStats = new();
        //    foreach (CreatureConfig companion in ActiveMemberAssets)
        //        activeCompanionsStats.Add(companion.StatsAsset);
        //    ES3.Save(id + "_activeCompanionsStats", activeCompanionsStats);
        //}

        //private void SaveMainCharacter(string id)
        //{
        //    ES3.Save(id + "_mainCharData", MainCharacterAsset);
        //    ES3.Save(id + "_mainCharacterStats", MainCharacterAsset.StatsAsset);
        //}

        public void LoadRuntimeData()
        {
            //Debug.Log("Loading runtime data");
            //string id = GetComponent<UniqueId>().Id;
            //MainCharacterAsset = ES3.Load<CreatureConfig>(id + "_mainCharData");
            ////MainCharacterAsset.StatsAsset = ES3.Load<CharacterStats>(id + "_mainCharacterStats");
            //ActiveMemberAssets = ES3.Load<List<CreatureConfig>>(id + "_activeCompanionsData");
            //List<CharacterStats> activeStats = ES3.Load<List<CharacterStats>>(id + "_activeCompanionsStats");
            //for (int i = 0; i < ActiveMemberAssets.Count; i++)
            //{
            //    //ActiveMemberAssets[i].StatsAsset = activeStats[i];
            //}
            //RecruitedMemberAssets = ES3.Load<List<CreatureConfig>>(id + "_recruitedCompanionsData");
            //List<CharacterStats> recruitedStats = ES3.Load<List<CharacterStats>>(id + "_recruitedCompanionsStats");
            //for (int i = 0; i < RecruitedMemberAssets.Count; i++)
            //{
            //    //RecruitedMemberAssets[i].StatsAsset = recruitedStats[i];
            //}
            //ActivePartySavedPositions = ES3.Load<List<Vector3>>(id + "_activePartyPositions");
        }

        //public GameDataContainer GetCurrentJournalData()
        //{
        //    return new GameDataContainer.GameDataContainerBuilder()
        //        .SetMainCharacterAsset(MainCharacterAsset)
        //        .SetActiveCompanionAssets(ActiveMemberAssets)
        //        .SetRecruitedCompanionAssets(RecruitedMemberAssets)
        //        .Build();
        //}

        //public void InjectJournalData(GameDataContainer data)
        //{
        //    MainCharacterAsset = data.mainCharacterAsset;
        //    ActiveMemberAssets = new List<CreatureConfig>(data.ActiveCompanionAssets);
        //    RecruitedMemberAssets = new List<CreatureConfig>(data.RecruitedCompanionAssets);
        //}

        //public static List<Creature> GetActivePartyCreatures()
        //{
        //    List<Creature> result = new List<Creature>
        //    {
        //        Instance.MainCharacterData.CreatureComponent
        //    };
        //    Instance.ActivePartyData.ForEach(c => result.Add(c.CreatureComponent));
        //    return result;
        //}
    }

    public class GameDataContainer
    {
        public readonly CreatureConfig mainCharacterAsset;
        private readonly List<CreatureConfig> _activeCompanionAssets;
        public ReadOnlyCollection<CreatureConfig> ActiveCompanionAssets => _activeCompanionAssets.AsReadOnly();
        private readonly List<CreatureConfig> _recruitedCompanionAssets;
        public ReadOnlyCollection<CreatureConfig> RecruitedCompanionAssets => _recruitedCompanionAssets.AsReadOnly();

        private GameDataContainer(CreatureConfig mainCharacterAsset, List<CreatureConfig> activeCompanionAssets, List<CreatureConfig> recruitedCompanionAssets)
        {
            this.mainCharacterAsset = mainCharacterAsset;
            _activeCompanionAssets = activeCompanionAssets;
            _recruitedCompanionAssets = recruitedCompanionAssets;
        }

        public class GameDataContainerBuilder
        {
            private CreatureConfig mainCharacterAsset;
            private List<CreatureConfig> activeCompanionAssets;
            private List<CreatureConfig> recruitedCompanionAssets;

            public GameDataContainerBuilder SetMainCharacterAsset(CreatureConfig mainCharacterAsset)
            {
                this.mainCharacterAsset = mainCharacterAsset;
                return this;
            }

            public GameDataContainerBuilder SetActiveCompanionAssets(List<CreatureConfig> activeCompanionAssets)
            {
                this.activeCompanionAssets = activeCompanionAssets;
                return this;
            }

            public GameDataContainerBuilder SetRecruitedCompanionAssets(List<CreatureConfig> recruitedCompanionAssets)
            {
                this.recruitedCompanionAssets = recruitedCompanionAssets;
                return this;
            }

            public GameDataContainer Build()
            {
                GameDataContainer data = new(mainCharacterAsset, activeCompanionAssets, recruitedCompanionAssets);
                return data;
            }
        }
    }
}
