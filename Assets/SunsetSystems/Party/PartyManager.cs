using Entities.Characters;
using UI.CharacterPortraits;
using UnityEngine;
using SunsetSystems.Utils;
using System.Collections.Generic;
using SunsetSystems.Loading;
using System;
using System.Linq;

namespace SunsetSystems.Party
{
    public class PartyManager : InitializedSingleton<PartyManager>, ISaveRuntimeData
    {
        [field: SerializeField]
        private SerializableStringCreatureInstanceDictionary _activeParty;
        public static Creature MainCharacter => Instance._activeParty[Instance.ActiveCoterieMemberKeys[0]];
        public static List<Creature> ActiveParty => Instance._activeParty.Values.ToList();
        [field: SerializeField]
        public List<string> ActiveCoterieMemberKeys { get; private set; } = new();
        [SerializeField]
        private SerializableStringCreatureAssetDictionary _recruitedCharacters;

        private PartyPortraitsController _partyPortraits;
        private PartyPortraitsController PartyPortraits
        {
            get
            {
                if (!_partyPortraits)
                    _partyPortraits = this.FindFirstComponentWithTag<PartyPortraitsController>(TagConstants.PARTY_PORTRAITS_CONTROLLER);
                return _partyPortraits;
            }
        }

        public override void Initialize()
        {
            CreatePartyList();
            PartyPortraits.Clear();
            Debug.Log("Party members count: " + _activeParty.Count);
            foreach (string key in ActiveCoterieMemberKeys)
            {
                PartyPortraits.AddPortrait(_activeParty[key].GetCreatureUIData());
            }
        }

        public static void InitializePartyAtPosition(Vector3 position)
        {

        }

        public static void InitializePartyAtPositions(List<Vector3> positions)
        {
            for (int i = 0; i < Instance.ActiveCoterieMemberKeys.Count; i++)
            {
                Vector3 position = Vector3.zero;
                try
                {
                    position = positions?[i] ?? Vector3.zero;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.LogException(e);
                }
                InitializePartyMember(Instance._recruitedCharacters[Instance.ActiveCoterieMemberKeys[i]], position);
            }
        }

        protected static Creature InitializePartyMember(CreatureData data, Vector3 position)
        {
            return CreatureInitializer.InitializeCreature(data, position);
        }

        public static void RecruitCharacter(CreatureData creatureData)
        {
            Instance._recruitedCharacters.Add(creatureData.FullName, creatureData);
        }

        private void CreatePartyList()
        {

        }

        public void SaveRuntimeData()
        {
            throw new System.NotImplementedException();
        }

        public void LoadRuntimeData()
        {
            throw new System.NotImplementedException();
        }
    }

    [Serializable]
    public class SerializableStringCreatureAssetDictionary : SerializableDictionary<string, CreatureData>
    {

    }

    [Serializable]
    public class SerializableStringCreatureInstanceDictionary : SerializableDictionary<string, Creature>
    {

    }
}
