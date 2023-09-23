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
    public class PartyManager : SerializedMonoBehaviour, ISaveable, IResetable
    {
        public static PartyManager Instance { get; private set; }

        [SerializeField]
        private Dictionary<string, ICreature> _activeParty = new();
        public ICreature MainCharacter => _activeParty.TryGetValue(Instance._mainCharacterKey, out ICreature creature) ? creature : null;
        public List<ICreature> ActiveParty => _activeParty.Values.ToList();
        public List<ICreature> Companions => _activeParty.Where(kv => kv.Key != Instance._mainCharacterKey).Select(kv => kv.Value).ToList();
        [SerializeField]
        private HashSet<string> _activeCoterieMemberKeys = new();
        [SerializeField]
        private Dictionary<string, CreatureData> _creatureDataCache = new();
        public List<CreatureData> AllCoterieMembers => _creatureDataCache.Values.ToList();
        [SerializeField, ReadOnly]
        private string _mainCharacterKey;

        public static Action<string, CreatureData> OnPartyMemberRecruited;
        public static Action<IEnumerable<string>> OnActivePartyChanged;

        [SerializeField, Required]
        private UniqueId _unique;
        public string DataKey => _unique.Id;

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

        protected void Awake()
        {
            Instance = this;
            _unique ??= GetComponent<UniqueId>();
            ISaveable.RegisterSaveable(this);
        }

        protected void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
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

        public void InitializePartyAtPosition(Vector3 position)
        {
            foreach (string key in Instance._activeCoterieMemberKeys)
            {
                CreatureData data = Instance._creatureDataCache[key];
                Instance._activeParty.Add(key, InitializePartyMember(data, position));
            }
        }

        public void InitializePartyAtPositions(List<Vector3> positions)
        {
            int index = 0;
            foreach (string key in Instance._activeCoterieMemberKeys)
            {
                CreatureData data = _creatureDataCache[key];
                Vector3 position = positions[index];
                _activeParty.Add(key, InitializePartyMember(data, position));
                index++;
            }
        }

        protected Creature InitializePartyMember(CreatureData data, Vector3 position)
        {
            if (data == null)
            {
                throw new NullReferenceException("Party member initialization failed! Null CreatureData!");
            }
            else
            {
                //Creature creature = CreatureInitializer.InitializeCreature(data, position, Instance._creatureParent, Quaternion.identity);
                //return creature;
            }
            return null;
        }

        public void RecruitCharacter(CreatureData creatureData)
        {
            Debug.Log($"Recruited {creatureData.FullName} to party!");
            _creatureDataCache.Add(creatureData.FullName, creatureData);
            ExperienceManager.AddCreatureToExperienceManager(creatureData.FullName);
            OnPartyMemberRecruited?.Invoke(creatureData.FullName, creatureData);
        }

        [Button]
        public void RecruitCharacter(ICreature creature)
        {
            RecruitCharacter(creature.References.CreatureData);
        }

        [Button]
        public void RecruitMainCharacter(CreatureData mainCharacterData)
        {
            RecruitCharacter(mainCharacterData);
            _mainCharacterKey = mainCharacterData.FullName;
            InventoryManager.Instance.SetMoney(0);
            if (TryAddMemberToActiveRoster(_mainCharacterKey) == false)
                Debug.LogError("Trying to recruit Main Character but Main Character already exists!");
        }

        public bool TryAddMemberToActiveRoster(string memberID)
        {
            if (_creatureDataCache.ContainsKey(memberID) == false)
                Debug.LogError("Trying to add character to roster but character " + memberID + " is not yet recruited!");
            bool result = _activeCoterieMemberKeys.Add(memberID);
            if (result)
                OnActivePartyChanged?.Invoke(_activeCoterieMemberKeys);
            return result;
        }

        [Button]
        public void AddCreatureAsActivePartyMember(ICreature creature)
        {
            if (TryAddMemberToActiveRoster(creature.References.CreatureData.FullName))
            {
                _activeParty.Add(creature.References.CreatureData.FullName, creature);
            }
        }

        public bool TryRemoveMemberFromActiveRoster(string memberID)
        {
            if (memberID.Equals(_mainCharacterKey))
            {
                Debug.LogError("Cannot remove Main Character from active roster!");
                return false;
            }
            bool result = _activeCoterieMemberKeys.Remove(memberID);
            if (result)
                OnActivePartyChanged?.Invoke(_activeCoterieMemberKeys);
            return result;
        }

        public bool UpdateCreatureData(CreatureData data)
        {
            if (_activeCoterieMemberKeys.Contains(data.FullName))
            {
                _creatureDataCache[data.FullName] = data;
                return true;
            }
            else
            {
                Debug.LogWarning("No cached party member with name " + data.FullName + " found!");
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
