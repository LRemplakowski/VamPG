using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SunsetSystems.Data;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Creatures;
using SunsetSystems.Experience;
using SunsetSystems.Inventory;
using SunsetSystems.Persistence;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Party
{
    public class PartyManager : SerializedMonoBehaviour, ISaveable, IResetable
    {
        public static PartyManager Instance { get; private set; }

        [field: SerializeField, ReadOnly, Title("Persistence")]
        public string DataKey { get; private set; }

        [Title("References")]
        [SerializeField]
        private Transform _creatureParent;

        [Title("Runtime")]
        [SerializeField, ReadOnly]
        private Dictionary<string, ICreature> _activeParty = new();
        public ICreature MainCharacter => _activeParty.TryGetValue(Instance._mainCharacterKey, out ICreature creature) ? creature : null;
        public List<ICreature> ActiveParty => _activeParty.Values.ToList();
        public List<ICreature> Companions => _activeParty.Where(kv => kv.Key != Instance._mainCharacterKey).Select(kv => kv.Value).ToList();
        [SerializeField, ReadOnly]
        private HashSet<string> _activeCoterieMemberKeys = new();
        [SerializeField, ReadOnly]
        private HashSet<string> _coterieMemberKeysCache = new();
        public List<string> AllCoterieMembers => _coterieMemberKeysCache.ToList();
        [SerializeField, ReadOnly]
        private string _mainCharacterKey;

        private Dictionary<string, ICreatureTemplate> _cachedPartyTemplates = new();
        private Dictionary<string, Vector3> _partyPositions = null;

        [Title("Events")]
        public UltEvent<IEnumerable<ICreature>> OnActivePartyInitialized = new();

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(DataKey))
                DataKey = Guid.NewGuid().ToString();
        }

        public void ResetOnGameStart()
        {
            _activeParty = new();
            _coterieMemberKeysCache = new();
            _mainCharacterKey = "";
            _activeCoterieMemberKeys = new();
        }

        protected void Awake()
        {
            Instance = this;
            ISaveable.RegisterSaveable(this);
            _activeCoterieMemberKeys.AddRange(_activeParty.Keys);
        }

        protected void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
        }

        public ICreature GetPartyMemberByID(string key)
        {
            return _activeParty[key];
        }

        public bool IsRecruitedMember(string key)
        {
            return _coterieMemberKeysCache.Contains(key);
        }

        public async void InitializePartyAtPosition(Vector3 position)
        {
            foreach (string key in Instance._activeCoterieMemberKeys)
            {
                ICreatureTemplate data = _cachedPartyTemplates[key];
                _activeParty.Add(key, await InitializePartyMember(data, position));
            }
            OnActivePartyInitialized?.InvokeSafe(_activeParty.Values.ToList());
        }

        public async void InitializePartyAtPositions(List<Vector3> positions)
        {
            int index = 0;
            foreach (string key in Instance._activeCoterieMemberKeys)
            {
                ICreatureTemplate data = _cachedPartyTemplates[key];
                Vector3 position = positions[index];
                _activeParty.Add(key, await InitializePartyMember(data, position));
                index++;
            }
            OnActivePartyInitialized?.InvokeSafe(_activeParty.Values.ToList());
        }

        private async Task<ICreature> InitializePartyMember(ICreatureTemplate data, Vector3 position)
        {
            if (data == null)
            {
                throw new NullReferenceException("Party member initialization failed! Null CreatureData!");
            }
            else
            {
                ICreature memberInstance = await CreatureFactory.Instance.Create(data);
                memberInstance.Transform.SetParent(_creatureParent);
                memberInstance.ForceToPosition(position);
                return memberInstance;
            }
        }

        public void UpdatePartyMemberTemplateFromInstance(ICreature memberInstance)
        {
            _cachedPartyTemplates[memberInstance.ID] = memberInstance.CreatureTemplate;
        }    

        public void RecruitCharacter(ICreatureTemplate memberTemplate)
        {
            Debug.Log($"Recruited {memberTemplate.FullName} to party!");
            _coterieMemberKeysCache.Add(memberTemplate.DatabaseID);
            _cachedPartyTemplates.Add(memberTemplate.DatabaseID, memberTemplate);
            ExperienceManager.AddCreatureToExperienceManager(memberTemplate.FullName);
        }

        [Title("Editor Utility")]
        [Button]
        public void RecruitCharacter(ICreature creature)
        {
            Debug.Log($"Recruited {creature.References.CreatureData.FullName} to party!");
            _coterieMemberKeysCache.Add(creature.ID);
            TryAddMemberToActiveRoster(creature.ID, creature);
            ExperienceManager.AddCreatureToExperienceManager(creature.ID);
        }

        [Button]
        public void RecruitMainCharacter(ICreatureTemplate mainCharTemplate)
        {
            _mainCharacterKey = mainCharTemplate.DatabaseID;
            RecruitCharacter(mainCharTemplate);
            _activeCoterieMemberKeys.Add(_mainCharacterKey);
            InventoryManager.Instance.SetMoney(0);
        }

        public bool TryAddMemberToActiveRoster(string memberID, ICreature creature)
        {
            bool result = _activeCoterieMemberKeys.Add(memberID);
            _activeParty.Add(memberID, creature);
            return result;
        }

        public bool TryRemoveMemberFromActiveRoster(string memberID)
        {
            if (memberID.Equals(_mainCharacterKey))
            {
                Debug.LogError("Cannot remove Main Character from active roster!");
                return false;
            }
            bool result = _activeCoterieMemberKeys.Remove(memberID);
            if (_activeParty.TryGetValue(memberID, out ICreature memberInstance))
            {
                UpdatePartyMemberTemplateFromInstance(memberInstance);
                CreatureFactory.Instance.DestroyCreature(memberInstance);
            }
            return result;
        }

        public object GetSaveData()
        {
            PartySaveData saveData = new();
            saveData.CachedPartyTemplates = new(_cachedPartyTemplates);
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
            _coterieMemberKeysCache = new();
            _cachedPartyTemplates = new();
            saveData.CachedPartyTemplates.Keys.ToList().ForEach(key => _cachedPartyTemplates.Add(key, saveData.CachedPartyTemplates[key]));
            _coterieMemberKeysCache.AddRange(_cachedPartyTemplates.Keys);
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
        public Dictionary<string, ICreatureTemplate> CachedPartyTemplates;
        public HashSet<string> ActiveMemberKeys;
        public string MainCharacterKey;
    }
}
