using SunsetSystems.Entities.Characters;
using UI.CharacterPortraits;
using UnityEngine;
using System.Collections.Generic;
using SunsetSystems.LevelManagement;
using System;
using System.Linq;
using CleverCrow.Fluid.UniqueIds;
using NaughtyAttributes;
using SunsetSystems.Inventory;
using SunsetSystems.Experience;
using SunsetSystems.Data;
using Zenject;

namespace SunsetSystems.Party
{
    [RequireComponent(typeof(UniqueId))]
    public class PartyManager : MonoBehaviour, ISaveable, IResetable, IPartyManager
    {
        public Creature MainCharacter => _activeParty[_mainCharacterKey];
        public List<Creature> ActiveParty => _activeParty.Values.ToList();
        public List<Creature> Companions => _activeParty.Where(kv => kv.Key != _mainCharacterKey).Select(kv => kv.Value).ToList();
        public List<CreatureData> AllCoterieMembers => new(_creatureDataCache.Values);

        [field: SerializeField]
        private StringCreatureInstanceDictionary _activeParty;
        private HashSet<string> _activeCoterieMemberKeys = new();
        [SerializeField]
        private StringCreatureDataDictionary _creatureDataCache;

        private string _mainCharacterKey;

        public static Action<string, CreatureData> OnPartyMemberRecruited;

        [SerializeField, Required]
        private UniqueId _unique;
        public string DataKey => _unique.Id;

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

        // DEPENDENCIES
        private IInventoryManager _inventoryManager;
        private IExperienceManager _experienceManager;

        [SerializeField]
        private Transform _creatureParent;

        [Inject]
        public void InjectDependencies(IInventoryManager inventoryManager, IExperienceManager experienceManager)
        {
            _inventoryManager = inventoryManager;
            _experienceManager = experienceManager;
        }

        public void ResetOnGameStart()
        {
            _activeParty = new();
            _creatureDataCache = new();
            _mainCharacterKey = "";
            _activeCoterieMemberKeys = new();
        }

        protected void Awake()
        {
            _mainCharacterKey = string.Empty;
            _activeParty = new();
            _creatureDataCache = new();
            _activeCoterieMemberKeys = new();
            _unique ??= GetComponent<UniqueId>();
            ISaveable.RegisterSaveable(this);
        }

        private void OnEnable()
        {
            LevelLoader.OnAfterLevelLoad += OnAfterLevelLoad;
        }

        private void OnDisable()
        {
            LevelLoader.OnAfterLevelLoad -= OnAfterLevelLoad;
        }

        private void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
        }

        private void OnAfterLevelLoad(LevelLoadingEventData data)
        {
            Waypoint entryPoint = this.FindFirstComponentWithTag<Waypoint>(data.AreaEntryPointTag);
            if (entryPoint)
                InitializePartyAtPosition(entryPoint.transform.position);
        }

        //TODO: Invoke portrait update as an event on active party list change
        private void UpdatePartyPortraits()
        {
            PartyPortraits?.Clear();
            foreach (string key in _activeCoterieMemberKeys)
            {
                PartyPortraits?.AddPortrait(_creatureDataCache[key].Portrait);
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

        public void InitializePartyAtPosition(Vector3 position)
        {
            foreach (string key in _activeCoterieMemberKeys)
            {
                CreatureData data = _creatureDataCache[key];
                _activeParty.Add(key, InitializePartyMember(data, position));
            }
        }

        //public void InitializePartyAtPositions(List<Vector3> positions)
        //{
        //    int index = 0;
        //    foreach (string key in _activeCoterieMemberKeys)
        //    {
        //        CreatureData data = _creatureDataCache[key];
        //        Vector3 position = positions[index];
        //        _activeParty.Add(key, InitializePartyMember(data, position));
        //        index++;
        //    }
        //}

        protected Creature InitializePartyMember(CreatureData data, Vector3 position)
        {
            Creature creature = CreatureInitializer.InitializeCreature(data, position);
            creature.transform.SetParent(_creatureParent, true);
            return creature;
        }

        public void RecruitCharacter(CreatureData creatureData)
        {
            Debug.Log($"Recruited {creatureData.ID} to party!");
            _creatureDataCache.Add(creatureData.ID, creatureData);
            _inventoryManager.AddCoterieMemberEquipment(creatureData.ID, creatureData);
            _experienceManager.AddCreatureToExperienceManager(creatureData.ID);
            OnPartyMemberRecruited?.Invoke(creatureData.ID, creatureData);
        }

        public void RecruitMainCharacter(CreatureData mainCharacterData)
        {
            RecruitCharacter(mainCharacterData);
            _mainCharacterKey = mainCharacterData.ID;
            _inventoryManager.SetMoney(mainCharacterData.Money);
            if (TryAddMemberToActiveRoster(_mainCharacterKey) == false)
                Debug.LogError("Trying to recruit Main Character but Main Character already exists!");
        }

        private bool TryAddMemberToActiveRoster(string memberID)
        {
            if (_creatureDataCache.ContainsKey(memberID) == false)
                Debug.LogError("Trying to add character to roster but character " + memberID + " is not yet recruited!");
            bool result = _activeCoterieMemberKeys.Add(memberID);
            if (result)
                UpdatePartyPortraits();
            return result;
        }

        public void AddCreatureAsActivePartyMember(Creature creature)
        {
            if (TryAddMemberToActiveRoster(creature.Data.ID))
            {
                _activeParty.Add(creature.Data.ID, creature);
            }
        }

        private bool TryRemoveMemberFromActiveRoster(string memberID)
        {
            if (memberID.Equals(_mainCharacterKey))
            {
                Debug.LogError("Cannot remove Main Character from active roster!");
                return false;
            }
            bool result = _activeCoterieMemberKeys.Remove(memberID);
            if (result)
                UpdatePartyPortraits();
            return result;
        }

        private void UpdateActivePartyData()
        {
            foreach (string key in _activeCoterieMemberKeys)
            {
                _activeParty[key].Data = _creatureDataCache[key];
            }
        }

        private bool UpdateCreatureData(CreatureData data)
        {
            if (_activeCoterieMemberKeys.Contains(data.ID))
            {
                _creatureDataCache[data.ID] = data;
                return true;
            }
            else
            {
                Debug.LogWarning("No cached party member with name " + data.ID + " found!");
                return false;
            }
        }

        public object GetSaveData()
        {
            PartySaveData saveData = new();
            saveData.CreatureDataCache = new(_creatureDataCache);
            saveData.ActiveMemberKeys = new(_activeCoterieMemberKeys);
            Dictionary<string, Vector3> partyPositions = new();
            foreach (string key in _activeParty.Keys)
            {
                partyPositions.Add(key, _activeParty[key].transform.position);
            }
            saveData.PartyPositions = partyPositions;
            return saveData;
        }

        public void InjectSaveData(object data)
        {
            PartySaveData saveData = data as PartySaveData;
            _creatureDataCache = new();
            saveData.CreatureDataCache.Keys.ToList().ForEach(key => _creatureDataCache.Add(key, saveData.CreatureDataCache[key]));
            _activeCoterieMemberKeys = saveData.ActiveMemberKeys;
            _activeParty = new();
            foreach (string key in _activeCoterieMemberKeys)
            {
                _activeParty.Add(key, InitializePartyMember(_creatureDataCache[key], saveData.PartyPositions[key]));
            }
        }
    }

    [Serializable]
    public class PartySaveData
    {
        public Dictionary<string, Vector3> PartyPositions;
        public Dictionary<string, CreatureData> CreatureDataCache;
        public HashSet<string> ActiveMemberKeys;
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
