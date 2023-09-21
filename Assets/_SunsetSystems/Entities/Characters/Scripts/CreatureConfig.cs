using UnityEngine;
using SunsetSystems.Entities.Data;
using SunsetSystems.Resources;
using SunsetSystems.Utils;
using Sirenix.OdinInspector;
using SunsetSystems.Utils.Extensions;
using SunsetSystems.Entities.Characters.Interfaces;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UMA;

namespace SunsetSystems.Entities.Characters
{
    [CreateAssetMenu(fileName = "New Creature Config", menuName = "Character/Creature Config")]
    public class CreatureConfig : SerializedScriptableObject, ICreatureTemplate
    {
        [field: SerializeField, ReadOnly]
        public string DatabaseID { get; private set; } = "";
        [SerializeField]
        private string _name = "New";
        public string FirstName { get => _name; }
        [SerializeField]
        private string _lastName = "Creature";
        public string LastName { get => _lastName; }
        public string FullName { get => $"{FirstName} {LastName}".Trim(); }
        [SerializeField]
        private bool _overrideReadableID;
        [SerializeField, ReadOnly, HideIf("_overrideReadableID")]
        private string _defaultReadableID = "";
        [SerializeField, ShowIf("_overrideReadableID")]
        private string _readableIDOverride = "";
        public string ReadableID => _overrideReadableID ? _readableIDOverride : _defaultReadableID;
        [field: SerializeField]
        public AssetReferenceSprite PortraitAssetRef { get; private set; }
        [SerializeField]
        private StatsConfig _statsConfig;
        public StatsData StatsData => new(_statsConfig);
        [field: SerializeField]
        public InventoryConfig EquipmentConfig { get; private set; }
        [field: SerializeField]
        public List<UMARecipeBase> BaseUmaRecipes { get; private set; }
        [SerializeField]
        private Faction _creatureFaction;
        public Faction Faction { get => _creatureFaction; }
        [SerializeField]
        private BodyType _bodyType;
        public BodyType BodyType { get => _bodyType; }
        [SerializeField]
        private CreatureType _creatureType;
        public CreatureType CreatureType { get => _creatureType; }
        [SerializeField]
        private bool _useEquipmentPreset;
        public bool UseEquipmentPreset => _useEquipmentPreset;
        [field: SerializeField]
        public EquipmentData EquipmentData { get; private set; }

        private void OnEnable()
        {
            //if (_statsConfig == null)
            //    _statsConfig = UnityEngine.Resources.Load<StatsConfig>("DEBUG/DebugStats");
            //if (EquipmentConfig == null)
            //    EquipmentConfig = UnityEngine.Resources.Load<InventoryConfig>("DEBUG/Default Inventory");
#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(DatabaseID))
            {
                AssignNewID();
            }
            if (string.IsNullOrWhiteSpace(DatabaseID) == false && CreatureDatabase.Instance?.IsRegistered(this) == false)
                CreatureDatabase.Instance?.RegisterConfig(this);
#endif
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(DatabaseID))
            {
                AssignNewID();
            }
            _defaultReadableID = FullName.ToCamelCase();
            if (string.IsNullOrWhiteSpace(DatabaseID) == false && CreatureDatabase.Instance?.IsRegistered(this) == false)
                CreatureDatabase.Instance?.RegisterConfig(this);
        }

        private void Reset()
        {
            AssignNewID();
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            CreatureDatabase.Instance.UnregisterConfig(this);
#endif
        }

        private void AssignNewID()
        {
            DatabaseID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
