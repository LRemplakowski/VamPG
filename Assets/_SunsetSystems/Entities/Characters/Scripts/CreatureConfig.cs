using UnityEngine;
using NaughtyAttributes;
using SunsetSystems.Entities.Data;
using SunsetSystems.Resources;
using SunsetSystems.Utils;

namespace SunsetSystems.Entities.Characters
{
    [CreateAssetMenu(fileName = "New Creature Config", menuName = "Character/Creature Config")]
    public class CreatureConfig : ScriptableObject
    {
        [field: SerializeField, ReadOnly]
        public string DatabaseID { get; private set; }
        [SerializeField]
        private string _name = "New";
        public string Name { get => _name; }
        [SerializeField]
        private string _lastName = "Creature";
        public string LastName { get => _lastName; }
        public string FullName { get => $"{Name} {LastName}".Trim(); }
        [SerializeField]
        private bool _overrideReadableID;
        [SerializeField, ReadOnly, HideIf("_overrideReadableID")]
        private string _defaultReadableID;
        [SerializeField, ShowIf("_overrideReadableID")]
        private string _readableIDOverride;
        public string ReadableID => _overrideReadableID ? _readableIDOverride : _defaultReadableID;
        [SerializeField]
        private Sprite _portrait;
        public Sprite Portrait { get => _portrait; }
        [SerializeField]
        private StatsConfig _statsConfig;
        public StatsConfig StatsAsset { get => _statsConfig; }
        [field: SerializeField]
        public InventoryConfig EquipmentConfig { get; private set; }
        [field: SerializeField]
        public TextAsset UmaPreset { get; private set; }
        [SerializeField]
        private RuntimeAnimatorController _animatorController;
        public RuntimeAnimatorController AnimatorController => _animatorController;
        [SerializeField]
        private Faction _creatureFaction;
        public Faction CreatureFaction { get => _creatureFaction; }
        [SerializeField]
        private BodyType _bodyType;
        public BodyType BodyType { get => _bodyType; }
        [SerializeField]
        private CreatureType _creatureType;
        public CreatureType CreatureType { get => _creatureType; }
        [SerializeField]
        private bool _useEquipmentPreset;
        public bool UseEquipmentPreset => _useEquipmentPreset;

        private void OnEnable()
        {
            if (_portrait == null)
                _portrait = UnityEngine.Resources.Load<Sprite>("DEBUG/missing");
            if (_statsConfig == null)
                _statsConfig = UnityEngine.Resources.Load<StatsConfig>("DEBUG/DebugStats");
            if (EquipmentConfig == null)
                EquipmentConfig = UnityEngine.Resources.Load<InventoryConfig>("DEBUG/Default Inventory");
            if (_animatorController == null)
                _animatorController = ResourceLoader.GetFallbackAnimator();
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
