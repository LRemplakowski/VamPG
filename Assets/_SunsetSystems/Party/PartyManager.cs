using SunsetSystems.Entities.Characters;
using UI.CharacterPortraits;
using UnityEngine;
using SunsetSystems.Utils;
using System.Collections.Generic;
using SunsetSystems.LevelManagement;
using System;
using System.Linq;
using CleverCrow.Fluid.UniqueIds;
using NaughtyAttributes;
using SunsetSystems.Inventory;
using SunsetSystems.Experience;
using SunsetSystems.Dialogue;
using SunsetSystems.Data;
using UMA;

namespace SunsetSystems.Party
{
    [RequireComponent(typeof(UniqueId))]
    public class PartyManager : InitializedSingleton<PartyManager>, ISaveable, IResetable
    {
        [field: SerializeField]
        private StringCreatureInstanceDictionary _activeParty;
        public static Creature MainCharacter => Instance._activeParty[Instance._mainCharacterKey];
        public static List<Creature> ActiveParty => Instance._activeParty.Values.ToList();
        public static List<Creature> Companions => Instance._activeParty.Where(kv => kv.Key != Instance._mainCharacterKey).Select(kv => kv.Value).ToList();
        private HashSet<string> _activeCoterieMemberKeys = new();
        [SerializeField]
        private StringCreatureDataDictionary _creatureDataCache;
        public static List<CreatureData> AllCoterieMembers => new(Instance._creatureDataCache.Values);

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

        [SerializeField]
        private Transform _creatureParent;

        public void ResetOnGameStart()
        {
            _activeParty = new();
            _creatureDataCache = new();
            _mainCharacterKey = "";
            _activeCoterieMemberKeys = new();
        }

        protected override void Awake()
        {
            base.Awake();
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

        public override void Initialize()
        {
            UpdatePartyPortraits();
        }

        public override void LateInitialize()
        {
            
        }

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
            Creature creature = CreatureInitializer.InitializeCreature(data, position);
            creature.transform.SetParent(Instance._creatureParent, true);
            return creature;
        }

        public static void RecruitCharacter(CreatureData creatureData)
        {
            Debug.Log($"Recruited {creatureData.ID} to party!");
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
            bool result = Instance._activeCoterieMemberKeys.Add(memberID);
            if (result)
                Instance.UpdatePartyPortraits();
            return result;
        }

        public static void AddCreatureAsActivePartyMember(Creature creature)
        {
            if (TryAddMemberToActiveRoster(creature.Data.ID))
            {
                Instance._activeParty.Add(creature.Data.ID, creature);
            }
        }

        public static bool TryRemoveMemberFromActiveRoster(string memberID)
        {
            if (memberID.Equals(Instance._mainCharacterKey))
            {
                Debug.LogError("Cannot remove Main Character from active roster!");
                return false;
            }
            bool result = Instance._activeCoterieMemberKeys.Remove(memberID);
            if (result)
                Instance.UpdatePartyPortraits();
            return result;
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
