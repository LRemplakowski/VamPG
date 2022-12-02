using SunsetSystems.Entities.Characters;
using UI.CharacterPortraits;
using UnityEngine;
using SunsetSystems.Utils;
using System.Collections.Generic;
using SunsetSystems.Loading;
using System;
using System.Linq;
using CleverCrow.Fluid.UniqueIds;
using NaughtyAttributes;
using SunsetSystems.Inventory;
using SunsetSystems.Experience;
using SunsetSystems.Dialogue;

namespace SunsetSystems.Party
{
    [RequireComponent(typeof(UniqueId))]
    public class PartyManager : InitializedSingleton<PartyManager>, ISaveRuntimeData
    {
        [field: SerializeField]
        private StringCreatureInstanceDictionary _activeParty;
        public static Creature MainCharacter => Instance._activeParty[Instance._mainCharacterKey];
        public static List<Creature> ActiveParty => Instance._activeParty.Values.ToList();
        public static List<Creature> Companions => Instance._activeParty.Where(kv => kv.Key != Instance._mainCharacterKey).Select(kv => kv.Value) as List<Creature>;
        private HashSet<string> _activeCoterieMemberKeys = new();
        [SerializeField]
        private StringCreatureDataDictionary _creatureDataCache;
        public static List<CreatureData> AllCoterieMembers => new(Instance._creatureDataCache.Values);

        private string _mainCharacterKey;

        public static Action<string, CreatureData> OnPartyMemberRecruited;

        [SerializeField, Required]
        private UniqueId _unique;

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

        protected override void Awake()
        {
            _mainCharacterKey = string.Empty;
            _activeParty = new();
            _creatureDataCache = new();
            _activeCoterieMemberKeys = new();
            _unique ??= GetComponent<UniqueId>();
        }

        public override void Initialize()
        {
            PartyPortraits?.Clear();
            Debug.Log("Party members count: " + _activeParty.Count);
            foreach (string key in _activeCoterieMemberKeys)
            {
                PartyPortraits?.AddPortrait(_activeParty[key].GetCreatureUIData());
            }
        }

        public Creature GetPartyMemberByID(string key)
        {
            return _activeParty[key];
        }

        public CreatureData GetPartyMemberDataByID(string key)
        {
            return _creatureDataCache[key];
        }

        public bool IsRecruitedMember(string key)
        {
            return _creatureDataCache.ContainsKey(key);
        }

        public static void InitializePartyAtPosition(Vector3 position)
        {
            foreach (string key in Instance._activeCoterieMemberKeys)
            {
                CreatureData data = Instance._creatureDataCache[key];
                Instance._activeParty.Add(key, InitializePartyMember(data, position));
            }
        }

        public static void InitializePartyAtPositions(List<Vector3> positions)
        {
            int index = 0;
            foreach (string key in Instance._activeCoterieMemberKeys)
            {
                CreatureData data = Instance._creatureDataCache[key];
                Vector3 position = positions[index];
                Instance._activeParty.Add(key, InitializePartyMember(data, position));
                index++;
            }
        }

        protected static Creature InitializePartyMember(CreatureData data, Vector3 position)
        {
            return CreatureInitializer.InitializeCreature(data, position);
        }

        public static void RecruitCharacter(CreatureData creatureData)
        {
            Instance._creatureDataCache.Add(creatureData.ID, creatureData);
            InventoryManager.AddCoterieMemberEquipment(creatureData.ID, creatureData);
            ExperienceManager.AddCreatureToExperienceManager(creatureData.ID);
            OnPartyMemberRecruited?.Invoke(creatureData.ID, creatureData);
        }

        public static void RecruitMainCharacter(CreatureData mainCharacterData)
        {
            RecruitCharacter(mainCharacterData);
            Instance._mainCharacterKey = mainCharacterData.ID;
            InventoryManager.Instance.SetMoney(mainCharacterData.Money);
            if (TryAddMemberToActiveRoster(Instance._mainCharacterKey) == false)
                Debug.LogError("Trying to recruit Main Character but Main Character already exists!");
        }

        public static bool TryAddMemberToActiveRoster(string memberID)
        {
            if (Instance._creatureDataCache.ContainsKey(memberID) == false)
                Debug.LogError("Trying to add character to roster but character " + memberID + " is not yet recruited!");
            return Instance._activeCoterieMemberKeys.Add(memberID);
        }

        public static bool TryRemoveMemberFromActiveRoster(string memberID)
        {
            if (memberID.Equals(Instance._mainCharacterKey))
            {
                Debug.LogError("Cannot remove Main Character from active roster!");
                return false;
            }
            return Instance._activeCoterieMemberKeys.Remove(memberID);
        }

        public static void UpdateActivePartyData()
        {
            foreach (string key in Instance._activeCoterieMemberKeys)
            {
                Instance._activeParty[key].Data = Instance._creatureDataCache[key];
            }
        }

        public bool UpdateCreatureData(CreatureData data)
        {
            if (Instance._activeCoterieMemberKeys.Contains(data.ID))
            {
                Instance._creatureDataCache[data.ID] = data;
                return true;
            }
            else
            {
                Debug.LogWarning("No cached party member with name " + data.ID + " found!");
                return false;
            }
        }

        public void SaveRuntimeData()
        {
            PartySaveData saveData = new();
            saveData.CreatureDataCache = _creatureDataCache;
            saveData.ActiveMemberKeys = _activeCoterieMemberKeys.ToList();
            StringVector3Dictionary partyPositions = new();
            foreach (string key in _activeParty.Keys)
            {
                partyPositions.Add(key, _activeParty[key].transform.position);
            }
            saveData.PartyPositions = partyPositions;
            ES3.Save(_unique.Id, saveData);
        }

        public void LoadRuntimeData()
        {
            PartySaveData saveData = ES3.Load<PartySaveData>(_unique.Id);
            _creatureDataCache = saveData.CreatureDataCache;
            _activeCoterieMemberKeys = saveData.ActiveMemberKeys.ToHashSet();
            _activeParty = new();
            foreach (string key in _activeCoterieMemberKeys)
            {
                _activeParty.Add(key, InitializePartyMember(_creatureDataCache[key], saveData.PartyPositions[key]));
            }
        }

        [Serializable]
        private struct PartySaveData
        {
            public StringVector3Dictionary PartyPositions;
            public StringCreatureDataDictionary CreatureDataCache;
            public List<string> ActiveMemberKeys;
        }
    }

    [Serializable]
    public class StringCreatureDataDictionary : SerializableDictionary<string, CreatureData>
    {

    }

    [Serializable]
    public class StringCreatureInstanceDictionary : SerializableDictionary<string, Creature>
    {

    }
}
