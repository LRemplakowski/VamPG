using UnityEngine;
using SunsetSystems.Entities.Data;
using Sirenix.OdinInspector;
using SunsetSystems.Utils.Extensions;
using SunsetSystems.Entities.Characters.Interfaces;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UMA;
using SunsetSystems.Equipment;
using System;
using SunsetSystems.Inventory.Data;

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
                EquipmentSlotsData = new();
                EquipmentSlotsData.Add(EquipmentSlotID.PrimaryWeapon, new EquipmentSlot(ItemCategory.WEAPON, EquipmentSlotID.PrimaryWeapon));
                EquipmentSlotsData.Add(EquipmentSlotID.SecondaryWeapon, new EquipmentSlot(ItemCategory.WEAPON, EquipmentSlotID.SecondaryWeapon));
                EquipmentSlotsData.Add(EquipmentSlotID.Chest, new EquipmentSlot(ItemCategory.CLOTHING, EquipmentSlotID.Chest));
                EquipmentSlotsData.Add(EquipmentSlotID.Boots, new EquipmentSlot(ItemCategory.SHOES, EquipmentSlotID.Boots));
                EquipmentSlotsData.Add(EquipmentSlotID.Hands, new EquipmentSlot(ItemCategory.GLOVES, EquipmentSlotID.Hands));
                EquipmentSlotsData.Add(EquipmentSlotID.Trinket, new EquipmentSlot(ItemCategory.TRINKET, EquipmentSlotID.Trinket));
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

        private void AssignNewID()
        {
            DatabaseID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        [Serializable]
        private class TemplateFromCreatureAsset : ICreatureTemplate
        {
            public string DatabaseID { get; }

            public string ReadableID { get; }

            public string FullName { get; }

            public string FirstName { get; }

            public string LastName { get; }

            public Faction Faction { get; }

            public BodyType BodyType { get; }

            public CreatureType CreatureType { get; }

            public AssetReferenceSprite PortraitAssetRef { get; }

            public List<UMARecipeBase> BaseUmaRecipes { get; }

            public Dictionary<EquipmentSlotID, IEquipmentSlot> EquipmentSlotsData { get; }

            public StatsData StatsData { get; }

            public TemplateFromCreatureAsset(CreatureConfig asset)
            {
                this.DatabaseID = asset.DatabaseID;
                this.ReadableID = asset.ReadableID;
                this.FullName = asset.FullName;
                this.FirstName = asset.FirstName;
                this.LastName = asset.LastName;
                this.Faction = asset.Faction;
                this.BodyType = asset.BodyType;
                this.CreatureType = asset.CreatureType;
                this.PortraitAssetRef = asset.PortraitAssetRef;
                this.BaseUmaRecipes = new(asset.BaseUmaRecipes);
                this.EquipmentSlotsData = new(asset.EquipmentSlotsData);
                this.StatsData = new(asset.StatsData);
            }

        }
    }
}
