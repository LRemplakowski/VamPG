using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Data;
using SunsetSystems.Equipment;
using SunsetSystems.Utils.Extensions;
using UMA.CharacterSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Entities.Characters
{
    [CreateAssetMenu(fileName = "New Creature Config", menuName = "Character/Creature Config")]
    public class CreatureConfig : SerializedScriptableObject, ICreatureTemplateProvider
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
        public UMAWardrobeCollection BaseLookWardrobeCollection { get; private set; }
        [SerializeField]
        private Faction _creatureFaction;
        public Faction Faction { get => _creatureFaction; }
        [SerializeField]
        private BodyType _bodyType;
        public BodyType BodyType { get => _bodyType; }
        [SerializeField]
        private CreatureType _creatureType;
        public CreatureType CreatureType { get => _creatureType; }
        [field: SerializeField, DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, IsReadOnly = true)]
        public Dictionary<EquipmentSlotID, IEquipmentSlot> EquipmentSlotsData { get; private set; }

        public ICreatureTemplate CreatureTemplate => new TemplateFromCreatureAsset(this);

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(DatabaseID))
            {
                AssignNewID();
            }
            if (string.IsNullOrWhiteSpace(DatabaseID) == false && CreatureDatabase.Instance?.IsRegistered(this) == false)
                CreatureDatabase.Instance?.RegisterConfig(this);
#endif
        }

        [Button("Force Validate")]
        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(DatabaseID))
            {
                AssignNewID();
            }
            _defaultReadableID = FullName.ToCamelCase();
            if (string.IsNullOrWhiteSpace(DatabaseID) == false && CreatureDatabase.Instance?.IsRegistered(this) == false)
                CreatureDatabase.Instance?.RegisterConfig(this);
            if (EquipmentSlotsData == null || EquipmentSlotsData.Count < Enum.GetValues(typeof(EquipmentSlotID)).Length - 1)
            {
                EquipmentSlotsData = new()
                {
                    { EquipmentSlotID.PrimaryWeapon, new EquipmentSlot(EquipmentSlotID.PrimaryWeapon) },
                    { EquipmentSlotID.SecondaryWeapon, new EquipmentSlot(EquipmentSlotID.SecondaryWeapon) },
                    { EquipmentSlotID.Chest, new EquipmentSlot(EquipmentSlotID.Chest) },
                    { EquipmentSlotID.Boots, new EquipmentSlot(EquipmentSlotID.Boots) },
                    { EquipmentSlotID.Hands, new EquipmentSlot(EquipmentSlotID.Hands) },
                    { EquipmentSlotID.Trinket, new EquipmentSlot(EquipmentSlotID.Trinket) }
                };
            }
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

        [Button("Randomize ID")]
        private void AssignNewID()
        {
            DatabaseID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        [Serializable]
        public class TemplateFromCreatureAsset : ICreatureTemplate
        {
            public string DatabaseID { get; private set; }

            public string ReadableID { get; private set; }

            public string FullName => $"{FirstName} {LastName}".Trim();

            public string FirstName { get; private set; }

            public string LastName { get; private set; }

            public Faction Faction { get; private set; }

            public BodyType BodyType { get; private set; }

            public CreatureType CreatureType { get; private set; }

            public AssetReferenceSprite PortraitAssetRef { get; private set; }

            public UMAWardrobeCollection BaseLookWardrobeCollection { get; private set; }

            public Dictionary<EquipmentSlotID, IEquipmentSlot> EquipmentSlotsData { get; private set; }

            public StatsData StatsData { get; private set; }

            public TemplateFromCreatureAsset(CreatureConfig asset)
            {
                this.DatabaseID = asset.DatabaseID;
                this.ReadableID = asset.ReadableID;
                this.FirstName = asset.FirstName;
                this.LastName = asset.LastName;
                this.Faction = asset.Faction;
                this.BodyType = asset.BodyType;
                this.CreatureType = asset.CreatureType;
                this.PortraitAssetRef = asset.PortraitAssetRef;
                this.BaseLookWardrobeCollection = asset.BaseLookWardrobeCollection;
                this.EquipmentSlotsData = new(asset.EquipmentSlotsData);
                this.StatsData = new(asset.StatsData);
            }

            public TemplateFromCreatureAsset()
            {

            }
        }
    }
}
