using SunsetSystems.Entities.Characters;
using UI.CharacterPortraits;
using UnityEngine;
using SunsetSystems.Utils;
using System.Collections.Generic;
using SunsetSystems.Persistence;
using System;
using System.Linq;
using CleverCrow.Fluid.UniqueIds;
using Sirenix.OdinInspector;
using SunsetSystems.Inventory;
using SunsetSystems.Experience;
using SunsetSystems.Dialogue;
using SunsetSystems.Data;
using UMA;
using SunsetSystems.Entities.Characters.Interfaces;

namespace SunsetSystems.Party
{
    [RequireComponent(typeof(UniqueId))]
    public class PartyManager : InitializedSingleton<PartyManager>, ISaveable, IResetable
    {
        [field: SerializeField]
        private Dictionary<string, ICreature> _activeParty;
        public static ICreature MainCharacter => Instance._activeParty.TryGetValue(Instance._mainCharacterKey, out ICreature creature) ? creature : null;
        public static List<ICreature> ActiveParty => Instance._activeParty.Values.ToList();
        public static List<ICreature> Companions => Instance._activeParty.Where(kv => kv.Key != Instance._mainCharacterKey).Select(kv => kv.Value).ToList();
        private HashSet<string> _activeCoterieMemberKeys = new();
        [SerializeField]
        private Dictionary<string, CreatureData> _creatureDataCache;
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

        private Dictionary<string, Vector3> _partyPositions = null;

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

        protected override void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
            base.OnDestroy();
        }

        public override void Initialize()
        {

        }

        public override void LateInitialize()
        {
            UpdatePartyPortraits();
        }

        private void UpdatePartyPortraits()
        {
            PartyPortraits?.Clear();
            foreach (string key in _activeCoterieMemberKeys)
            {
                //PartyPortraits?.AddPortrait(_creatureDataCache[key].Portrait);
            }
        }

        public ICreature GetPartyMemberByID(string key)
        {
            return _activeParty[key];
        }

        public CreatureData GetPartyMemberDataByID(string key)
        {
            if (_creatureDataCache.TryGetValue(key, out CreatureData data))
                return data;
            else
                return new();
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
            if (data == null)
            {
                throw new NullReferenceException("Party member initialization failed! Null CreatureData!");
            }
            else
            {
                Creature creature = CreatureInitializer.InitializeCreature(data, position, Instance._creatureParent, Quaternion.identity);
                return creature;
            }
        }

        public static void RecruitCharacter(CreatureData creatureData)
        {
            Debug.Log($"Recruited {creatureData.DatabaseID} to party!");
            Instance._creatureDataCache.Add(creatureData.DatabaseID, creatureData);
            InventoryManager.AddCoterieMemberEquipment(creatureData.DatabaseID, creatureData);
            ExperienceManager.AddCreatureToExperienceManager(creatureData.DatabaseID);
            OnPartyMemberRecruited?.Invoke(creatureData.DatabaseID, creatureData);
        }

        public static void RecruitCharacter(ICreature creature)
        {

        }

        public static void RecruitMainCharacter(CreatureData mainCharacterData)
        {
            RecruitCharacter(mainCharacterData);
            Instance._mainCharacterKey = mainCharacterData.DatabaseID;
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

        public static void AddCreatureAsActivePartyMember(ICreature creature)
        {
            if (TryAddMemberToActiveRoster(creature.ID))
            {
                Instance._activeParty.Add(creature.ID, creature);
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

        public bool UpdateCreatureData(CreatureData data)
        {
            if (Instance._activeCoterieMemberKeys.Contains(data.DatabaseID))
            {
                Instance._creatureDataCache[data.DatabaseID] = data;
                return true;
            }
            else
            {
                Debug.LogWarning("No cached party member with name " + data.DatabaseID + " found!");
                return false;
            }
        }

        public object GetSaveData()
        {
            PartySaveData saveData = new();
            saveData.CreatureDataCache = new(_creatureDataCache);
            saveData.ActiveMemberKeys = new(_activeCoterieMemberKeys);
            saveData.MainCharacterKey = _mainCharacterKey;
            Dictionary<string, Vector3> partyPositions = new();
            foreach (string key in _activeParty.Keys)
            {
                partyPositions.Add(key, _activeParty[key].References.Transform.position);
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
            _mainCharacterKey = saveData.MainCharacterKey;
            _activeParty = new();
            _partyPositions = new();
            foreach (string key in _activeCoterieMemberKeys)
            {
                if (saveData.PartyPositions.TryGetValue(key, out Vector3 position))
                    _partyPositions.Add(key, position);
                else
                    _partyPositions.Add(key, Vector3.zero);
            }
        }
    }

    [Serializable]
    public class PartySaveData
    {
        public Dictionary<string, Vector3> PartyPositions;
        public Dictionary<string, CreatureData> CreatureDataCache;
        public HashSet<string> ActiveMemberKeys;
        public string MainCharacterKey;
    }
}
